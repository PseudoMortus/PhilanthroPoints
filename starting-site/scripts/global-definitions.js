(
    function () {
        var now = Date.now()
        window.initialTimestamps = {
            initialTimestamp: now,
            initialRequestTimestamp: Math.round(
                performance.timeOrigin ? performance.timeOrigin : now - performance.now()
            )
        }
        window.thunderboltTag = "libs-releases-GA-local"
        window.thunderboltVersion = "1.16393.0"
    }
)();