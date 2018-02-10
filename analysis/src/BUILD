cc_library(
    name = "libsimulator",
    srcs = [
        "main.cc",
        "QuizSimulator.cc",
        "StrategyBuilder.cc",
    ],
    hdrs = [
        "QuizSimulator.h",
        "QuizConfig.h",
        "Strategy.h",
        "StrategyBuilder.h",
    ]
)

cc_binary(
    name = "simulator.exe",
    deps = [
        ":libsimulator"
    ]
)