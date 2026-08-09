local nk = require("nakama")


-- ==========================================
-- CATALOGO OFICIAL DE LA TIENDA
-- ==========================================

local PRODUCTS = {

    {
        id = "cubo_2x2",
        name = "Cubo 2x2",
        category = "muebles",
        price = 250
    }

}


-- ==========================================
-- UTILIDADES
-- ==========================================

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


-- ==========================================
-- RPC: OBTENER CATALOGO
-- ==========================================

local function rpc_get_catalog(
    context,
    payload
)

    require_user(
        context
    )


    return nk.json_encode({

        products =
            PRODUCTS

    })
end


nk.register_rpc(
    rpc_get_catalog,
    "shop_get_catalog"
)


nk.logger_info(
    "Shop module loaded."
)