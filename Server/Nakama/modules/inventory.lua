local nk = require("nakama")

local INVENTORY_COLLECTION =
    "player_inventory"


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
                            == true
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


    return nk.json_encode(
        {
            items = items,
            count = #items
        }
    )
end


nk.register_rpc(
    rpc_get_inventory,
    "inventory_get"
)


nk.logger_info(
    "Inventory module loaded."
)