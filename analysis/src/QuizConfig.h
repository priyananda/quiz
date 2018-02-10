#pragma once
#include <memory>
#include "Strategy.h"

class QuizConfig
{
public:
    QuizConfig() = default;
    QuizConfig(const QuizConfig&) = delete;
    QuizConfig(QuizConfig&&) = default;
    QuizConfig& setTeams(int numTeams) {
        this->numTeams_ = numTeams;
        return *this;
    }
    QuizConfig& setQuestions(int numQuestions) {
        this->numQuestions_ = numQuestions;
        return *this;
    }
    QuizConfig& setStrategy(std::unique_ptr<Strategy>&& strategy) {
        this->strategy_ = std::move(strategy);
        return *this;
    }
    int numTeams() const {
        return this->numTeams_;
    }
    int numQuestions() const {
        return this->numQuestions_;
    }
    Strategy& strategy() const {
        return *strategy_;
    }

private:
    int numTeams_ = 6;
    int numQuestions_ = 50;
    std::unique_ptr<Strategy> strategy_;
};