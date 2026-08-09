local nk = require("nakama")

local INVENTORY_COLLECTION =
    "player_inventory"

local ROOM_WIDTH = 10
local ROOM_LENGTH = 10


local function require_user(context)

    local user_id =
        context.user_id

    if
        user_id == nil
        or
        user_id == ""
    then
        error(
            "Authentication required."
        )
    end

    return user_id
end


local function error_response(
    code,
    message
)

    return nk.json_encode({
        success = false,
        code = code,
        message = message
    })
end


-- =====================================================
-- OBTENER INVENTARIO
-- =====================================================

local function rpc_get_inventory(
    context,
    payload
)

    local user_id =
        require_user(
            context
        )


    local items = {}

    local cursor = ""


    repeat

        local objects,
            next_cursor =
            nk.storage_list(
                user_id,
                INVENTORY_COLLECTION,
                100,
                cursor
            )


        if objects ~= nil then

            for _, object
                in ipairs(objects)
            do

                local value =
                    object.value
                    or {}


                table.insert(
                    items,
                    {
                        item_id =
                            value.item_id
                            or object.key,

                        product_id =
                            value.product_id
                            or "",

                        name =
                            value.name
                            or "",

                        category =
                            value.category
                            or "",

                        acquired_at =
                            value.acquired_at
                            or 0,

                        source =
                            value.source
                            or "",

                        placed =
                            value.placed
                            == true,

                        room_id =
                            value.room_id
                            or "",

                        grid_x =
                            value.grid_x
                            or 0,

                        grid_z =
                            value.grid_z
                            or 0,

                        rotation_y =
                            value.rotation_y
                            or 0
                    }
                )

            end

        end


        cursor =
            next_cursor
            or ""


    until cursor == ""


    table.sort(
        items,
        function(a, b)

            return
                (a.acquired_at or 0)
                <
                (b.acquired_at or 0)

        end
    )


    return nk.json_encode({
        items = items,
        count = #items
    })
end


-- =====================================================
-- GUARDAR COLOCACION DE UN MUEBLE
-- =====================================================

local function rpc_place_item(
    context,
    payload
)

    local user_id =
        require_user(
            context
        )


    if
        payload == nil
        or
        payload == ""
    then
        return error_response(
            "invalid_payload",
            "Datos de colocacion invalidos."
        )
    end


    local correcto,
        data =
        pcall(
            nk.json_decode,
            payload
        )


    if
        not correcto
        or
        data == nil
    then
        return error_response(
            "invalid_payload",
            "Datos de colocacion invalidos."
        )
    end


    local item_id =
        data.item_id

    local room_id =
        data.room_id

    local grid_x =
        tonumber(
            data.grid_x
        )

    local grid_z =
        tonumber(
            data.grid_z
        )

    local rotation_y =
        tonumber(
            data.rotation_y
        )


    if
        item_id == nil
        or
        item_id == ""
    then
        return error_response(
            "invalid_item",
            "El mueble no tiene un identificador valido."
        )
    end


    if
        room_id == nil
        or
        room_id == ""
    then
        return error_response(
            "invalid_room",
            "Habitacion invalida."
        )
    end


    if
        grid_x == nil
        or
        grid_z == nil
    then
        return error_response(
            "invalid_position",
            "Posicion invalida."
        )
    end


    if
        grid_x % 1 ~= 0
        or
        grid_z % 1 ~= 0
        or
        grid_x < 0
        or
        grid_z < 0
        or
        grid_x >= ROOM_WIDTH
        or
        grid_z >= ROOM_LENGTH
    then
        return error_response(
            "invalid_position",
            "La casilla esta fuera de la habitacion."
        )
    end


    if
        rotation_y ~= 0
        and
        rotation_y ~= 90
        and
        rotation_y ~= 180
        and
        rotation_y ~= 270
    then
        return error_response(
            "invalid_rotation",
            "Rotacion invalida."
        )
    end


    -- El objeto se busca usando:
    -- usuario actual + item UUID.
    -- Así una cuenta no puede modificar
    -- muebles pertenecientes a otra cuenta.

    local objects =
        nk.storage_read({
            {
                collection =
                    INVENTORY_COLLECTION,

                key =
                    item_id,

                user_id =
                    user_id
            }
        })


    if
        objects == nil
        or
        #objects == 0
    then
        return error_response(
            "item_not_found",
            "El mueble no pertenece a esta cuenta."
        )
    end


    local object =
        objects[1]

    local value =
        object.value
        or {}


    value.placed =
        true

    value.room_id =
        room_id

    value.grid_x =
        grid_x

    value.grid_z =
        grid_z

    value.rotation_y =
        rotation_y

    value.placed_at =
        os.time()


    local write = {
        collection =
            INVENTORY_COLLECTION,

        key =
            item_id,

        user_id =
            user_id,

        value =
            value,

        version =
            object.version,

        permission_read =
            1,

        permission_write =
            0
    }


    local write_ok,
        write_error =
        pcall(
            nk.storage_write,
            {
                write
            }
        )


    if not write_ok then

        nk.logger_error(
            "Error guardando colocacion: " ..
            tostring(
                write_error
            )
        )


        return error_response(
            "storage_error",
            "No se pudo guardar la colocacion."
        )
    end


    nk.logger_info(
        "FURNITURE PLACED user=" ..
        user_id ..
        " item=" ..
        item_id ..
        " room=" ..
        room_id ..
        " x=" ..
        tostring(grid_x) ..
        " z=" ..
        tostring(grid_z) ..
        " rotation=" ..
        tostring(rotation_y)
    )


    return nk.json_encode({
        success = true,
        code = "ok",
        message = "Mueble colocado correctamente.",

        item_id = item_id,
        placed = true,

        room_id = room_id,
        grid_x = grid_x,
        grid_z = grid_z,
        rotation_y = rotation_y
    })
end


nk.register_rpc(
    rpc_get_inventory,
    "inventory_get"
)


nk.register_rpc(
    rpc_place_item,
    "inventory_place"
)


nk.logger_info(
    "Inventory module loaded."
)