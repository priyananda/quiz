#include "QuizSimulator.h"
#include <random>
#include <iostream>
#include <chrono>
#include <algorithm>

void QuizSimulator::run(int numTrials) const {
    while (numTrials > 0) {
        runSimulation();
        --numTrials;
    }
    for (int wins : teamWinnings_)
        std::cout << wins << "\t";
    std::cout << std::endl;
}

void QuizSimulator::runSimulation() const {
    std::vector<int> teamScores;
    teamScores.resize(config_.numTeams());
    int currentTeam = 0;
    std::default_random_engine generator;
    generator.seed(std::chrono::system_clock::now().time_since_epoch().count());
    std::uniform_int_distribution<int> distribution(0, 100);

    for (int question = 1; question <= config_.numQuestions(); ++question) {
        const int directTeam = currentTeam;
        bool questionAnswered = false;
        do {
            const Route route = ((currentTeam == directTeam) ? Route::Direct : Route::Pass);
            const int randomValue = distribution(generator);
            questionAnswered = checkAnswer(question, currentTeam, route, randomValue);
            teamScores[currentTeam] += config_.strategy().getPoints(
                currentTeam,
                question,
                questionAnswered,
                route
            );
            if (!questionAnswered)
                currentTeam = (currentTeam + 1) % config_.numTeams();
        } while(!questionAnswered && currentTeam != directTeam);
        currentTeam = config_.strategy().nextTeam(
            directTeam,
            questionAnswered ? currentTeam : -1
        );
        currentTeam %= config_.numTeams();
    }
    int winningScore = *(std::max_element(teamScores.begin(), teamScores.end()));
    for (int t = 0; t < config_.numTeams(); ++t)
        if (teamScores[t] == winningScore)
            teamWinnings_[t]++;
}

bool QuizSimulator::checkAnswer(int question, int currentTeam, Route route, int randomValue) const {
    const int probablityOfCorrect =  config_.strategy().getProbability(
        currentTeam,
        question,
        route
    );
    if (probablityOfCorrect == 0)
        return false;
    if (probablityOfCorrect == 100)
        return true;
    return (randomValue <= probablityOfCorrect);
}