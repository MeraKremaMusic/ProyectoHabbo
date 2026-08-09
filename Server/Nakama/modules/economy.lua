local nk = require("nakama")

local STARTING_COINS = 1000

local STATE_COLLECTION = "economy_state"
local STATE_KEY = "starter_wallet"

local function require_user(context)
    local user_id = context.user_id

    if user_id == nil or user_id == "" then
        error("Authentication required.")
    end

    return user_id
end


local function ensure_starter_wallet(user_id)

    local objects = nk.storage_read({
        {
            collection = STATE_COLLECTION,
            key = STATE_KEY,
            user_id = user_id
        }
    })

    -- Ya recibió sus monedas iniciales.
    if #objects > 0 then
        return
    end


    -- El marcador y las monedas se crean
    -- juntos desde el servidor.
    local storage_writes = {
        {
            collection = STATE_COLLECTION,
            key = STATE_KEY,
            user_id = user_id,

            value = {
                starter_granted = true,
                starting_coins = STARTING_COINS
            },

            -- "*" significa que este objeto
            -- debe ser nuevo.
            version = "*",

            -- Solo el servidor necesita verlo
            -- y modificarlo.
            permission_read = 0,
            permission_write = 0
        }
    }


    local wallet_updates = {
        {
            user_id = user_id,

            changeset = {
                coins = STARTING_COINS
            },

            metadata = {
                reason = "starter_grant"
            }
        }
    }


    local ok, err = pcall(function()

        nk.multi_update(
            {},
            storage_writes,
            {},
            wallet_updates,
            true
        )

    end)


    if not ok then

        -- Puede ocurrir si dos solicitudes intentaron
        -- inicializar la misma cuenta simultáneamente.
        -- Comprobamos si el otro proceso ya lo hizo.

        local check = nk.storage_read({
            {
                collection = STATE_COLLECTION,
                key = STATE_KEY,
                user_id = user_id
            }
        })

        if #check == 0 then
            error(
                "Could not initialize wallet: " ..
                tostring(err)
            )
        end
    end
end


local function rpc_get_wallet(context, payload)

    local user_id =
        require_user(context)

    ensure_starter_wallet(
        user_id
    )


    local account =
        nk.account_get_id(
            user_id
        )


    local coins = 0

    if
        account.wallet ~= nil
        and
        account.wallet.coins ~= nil
    then
        coins =
            account.wallet.coins
    end


    return nk.json_encode({
        coins = coins
    })
end


nk.register_rpc(
    rpc_get_wallet,
    "economy_get_wallet"
)


nk.logger_info(
    "Economy module loaded."
)