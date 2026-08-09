local nk = require("nakama")

-- =========================================================
-- RELOJ GLOBAL DEL MUNDO
-- =========================================================
-- Un ciclo completo dura 20 minutos reales.
-- Cambia SOLO este valor en el servidor si luego quieres
-- un dia mas corto o mas largo para todos los jugadores.
local CYCLE_SECONDS = 20 * 60

-- Punto de referencia global del ciclo.
-- Mantenerlo fijo hace que reiniciar Nakama NO reinicie el dia.
local CYCLE_EPOCH = 0

-- Preparado para ampliar luego a lluvia, nubes, etc.
local CURRENT_WEATHER = "clear"

local function require_user(context)
    local user_id = context.user_id

    if user_id == nil or user_id == "" then
        error("Authentication required.")
    end

    return user_id
end

local function positive_mod(value, divisor)
    return ((value % divisor) + divisor) % divisor
end

local function rpc_get_world_state(context, payload)
    require_user(context)

    local now = os.time()
    local elapsed = now - CYCLE_EPOCH
    local cycle_position = positive_mod(elapsed, CYCLE_SECONDS)
    local normalized_time = cycle_position / CYCLE_SECONDS
    local game_hour = normalized_time * 24

    return nk.json_encode({
        server_unix = now,
        cycle_seconds = CYCLE_SECONDS,
        cycle_epoch = CYCLE_EPOCH,
        normalized_time = normalized_time,
        game_hour = game_hour,
        weather = CURRENT_WEATHER
    })
end

nk.register_rpc(
    rpc_get_world_state,
    "world_get_state"
)

nk.logger_info(
    "World time module loaded. Cycle: " ..
    tostring(CYCLE_SECONDS) ..
    " seconds."
)
