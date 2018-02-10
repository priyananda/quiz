#include "QuizSimulator.h"
#include "StrategyBuilder.h"
#include <iostream>

static void testInfiniteBounce() {
    StrategyBuilder builder;
    builder
        .setTeamDistribution(TeamDistribution::Uniform)
        .setNegativesPolicy(NegativesPolicy::None)
        .setPoints(10)
        .setRotationPolicy(RotationPolicy::InfiniteBounce)
        .setRouteBias(RouteBias::None);

    QuizConfig config;
    config
        .setTeams(6)
        .setQuestions(10)
        .setStrategy(std::move(builder.build()));

    std::cout << "Infinite Bounce: ";
    QuizSimulator simulator(std::move(config));
    simulator.run(10000);
}

static void testRoundRobin() {
    StrategyBuilder builder;
    builder
        .setTeamDistribution(TeamDistribution::Uniform)
        .setNegativesPolicy(NegativesPolicy::None)
        .setPoints(10)
        .setRouteBias(RouteBias::None)
        .setRotationPolicy(RotationPolicy::RoundRobin);

    QuizConfig config;
    config
        .setTeams(6)
        .setQuestions(10)
        .setStrategy(std::move(builder.build()));

    std::cout << "Round Robin: ";
    QuizSimulator simulator(std::move(config));
    simulator.run(10000);
}

int main(int argc, char* argv[]) {
    testInfiniteBounce();
    testRoundRobin();
    return 0;
}