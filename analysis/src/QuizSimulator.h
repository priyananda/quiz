#pragma once
#include "QuizConfig.h"
#include <vector>

class QuizSimulator
{
public:
    QuizSimulator(QuizConfig&& config) :
        config_(std::move(config)) {
        teamWinnings_.resize(config_.numTeams());
    }

    void run(int numTrials) const;

private:
    void runSimulation() const;
    bool checkAnswer(int question, int currentTeam, Route route, int randomValue) const;

    const QuizConfig config_;
    mutable std::vector<int> teamWinnings_;
};
