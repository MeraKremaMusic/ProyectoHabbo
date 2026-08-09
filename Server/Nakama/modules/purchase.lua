local nk =
    require("nakama")

local catalog =
    require("shop_catalog")


local INVENTORY_COLLECTION =
    "player_inventory"

local LOCK_COLLECTION =
    "economy_lock"

local LOCK_KEY =
    "shop_purchase"

local MAX_RETRIES =
    3


local function require_user(
    context
)

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


local function respuesta_error(
    code,
    message,
    coins
)

    return {

        success = false,
        code = code,
        message = message,
        coins = coins or 0,
        product_id = "",
        item_id = "",
        price = 0

    }

end


local function obtener_saldo(
    user_id
)

    local account =
        nk.account_get_id(
            user_id
        )


    if
        account == nil
        or
        account.wallet == nil
        or
        account.wallet.coins == nil
    then

        return 0

    end


    return
        tonumber(
            account.wallet.coins
        )
        or 0

end


local function leer_lock(
    user_id
)

    local objects =
        nk.storage_read({

            {
                collection =
                    LOCK_COLLECTION,

                key =
                    LOCK_KEY,

                user_id =
                    user_id
            }

        })


    if
        objects ~= nil
        and
        #objects > 0
    then

        return objects[1]

    end


    return nil

end


local function intentar_compra(
    user_id,
    product
)

    -- ==========================================
    -- COMPROBAR DINERO ACTUAL
    -- ==========================================

    local coins =
        obtener_saldo(
            user_id
        )


    if coins < product.price then

        return
            respuesta_error(
                "insufficient_funds",
                "No tienes suficientes monedas.",
                coins
            ),
            nil

    end


    -- ==========================================
    -- LOCK DE ECONOMIA
    --
    -- Evita que dos compras simultaneas
    -- gasten las mismas monedas.
    -- ==========================================

    local lock =
        leer_lock(
            user_id
        )


    local lock_version =
        "*"

    local sequence =
        0


    if lock ~= nil then

        lock_version =
            lock.version

        if
            lock.value ~= nil
            and
            lock.value.sequence ~= nil
        then

            sequence =
                tonumber(
                    lock.value.sequence
                )
                or 0

        end

    end


    -- ==========================================
    -- CREAR UNIDAD UNICA DEL MUEBLE
    -- ==========================================

    local item_id =
        nk.uuid_v4()


    local item_value = {

        item_id =
            item_id,

        product_id =
            product.id,

        name =
            product.name,

        category =
            product.category,

        acquired_at =
            os.time(),

        source =
            "shop",

        placed =
            false

    }


    -- ==========================================
    -- ESCRITURAS ATOMICAS
    -- ==========================================

    local storage_writes = {

        -- Lock/version de la economia.
        {
            collection =
                LOCK_COLLECTION,

            key =
                LOCK_KEY,

            user_id =
                user_id,

            value = {
                sequence =
                    sequence + 1
            },

            version =
                lock_version,

            permission_read =
                0,

            permission_write =
                0
        },


        -- Mueble comprado.
        {
            collection =
                INVENTORY_COLLECTION,

            key =
                item_id,

            user_id =
                user_id,

            value =
                item_value,

            version =
                "*",

            permission_read =
                1,

            permission_write =
                0
        }

    }


    local wallet_updates = {

        {
            user_id =
                user_id,

            changeset = {
                coins =
                    -product.price
            },

            metadata = {

                reason =
                    "shop_purchase",

                product_id =
                    product.id,

                item_id =
                    item_id,

                price =
                    product.price
            }
        }

    }


    local correcto,
        error_compra =
        pcall(

            function()

                nk.multi_update(
                    {},
                    storage_writes,
                    {},
                    wallet_updates,
                    true
                )

            end

        )


    if not correcto then

        nk.logger_warn(
            "Purchase transaction retry: " ..
            tostring(
                error_compra
            )
        )


        return nil,
            "retry"

    end


    -- ==========================================
    -- SALDO FINAL
    -- ==========================================

    local nuevo_saldo =
        obtener_saldo(
            user_id
        )


    return {

        success =
            true,

        code =
            "ok",

        message =
            "Compra realizada correctamente.",

        product_id =
            product.id,

        item_id =
            item_id,

        price =
            product.price,

        coins =
            nuevo_saldo

    },
    nil

end


local function rpc_buy(
    context,
    payload
)

    local user_id =
        require_user(
            context
        )


    local data = {}


    if
        payload ~= nil
        and
        payload ~= ""
    then

        local correcto,
            resultado =
            pcall(
                nk.json_decode,
                payload
            )


        if correcto
            and
            resultado ~= nil
        then

            data =
                resultado

        end

    end


    local product_id =
        data.product_id


    if
        product_id == nil
        or
        product_id == ""
    then

        return nk.json_encode(

            respuesta_error(
                "invalid_product",
                "Producto invalido.",
                obtener_saldo(
                    user_id
                )
            )

        )

    end


    -- ==========================================
    -- PRECIO OFICIAL DEL SERVIDOR
    -- ==========================================

    local product =
        catalog.find(
            product_id
        )


    if product == nil then

        return nk.json_encode(

            respuesta_error(
                "product_not_found",
                "El producto no existe.",
                obtener_saldo(
                    user_id
                )
            )

        )

    end


    -- ==========================================
    -- INTENTAR COMPRA
    --
    -- Si dos compras llegan exactamente
    -- al mismo tiempo, la version del lock
    -- obliga a revalidar el saldo.
    -- ==========================================

    for intento = 1,
        MAX_RETRIES
    do

        local resultado,
            estado =
            intentar_compra(
                user_id,
                product
            )


        if resultado ~= nil then

            if resultado.success then

                nk.logger_info(
                    "PURCHASE OK user=" ..
                    user_id ..
                    " product=" ..
                    product.id ..
                    " item=" ..
                    resultado.item_id
                )

            end


            return nk.json_encode(
                resultado
            )

        end


        if estado ~= "retry" then

            break

        end

    end


    return nk.json_encode(

        respuesta_error(
            "purchase_busy",
            "No se pudo completar la compra. Intenta nuevamente.",
            obtener_saldo(
                user_id
            )
        )

    )

end


nk.register_rpc(
    rpc_buy,
    "shop_buy"
)


nk.logger_info(
    "Purchase module loaded."
)