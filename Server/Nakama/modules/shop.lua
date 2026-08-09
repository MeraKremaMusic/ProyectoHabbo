local nk =
    require("nakama")

local catalog =
    require("shop_catalog")


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


local function rpc_get_catalog(
    context,
    payload
)

    require_user(
        context
    )


    return nk.json_encode({

        products =
            catalog.get_all()

    })

end


nk.register_rpc(
    rpc_get_catalog,
    "shop_get_catalog"
)


nk.logger_info(
    "Shop catalog module loaded."
)