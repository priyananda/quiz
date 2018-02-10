#pragma once

// How this team got this question.
enum class Route {
    Direct,
    Pass
};

// Encapsulates different logical choices we can make in awarding points and
// choosing teams.
class Strategy
{
public:
    // How many points does {team} get on answering a {Direct/Pass}
    // question# {question} it is {isCorrect/!isCorrect}
    virtual int getPoints(
        int team,
        int question,
        bool isCorrect,
        Route route
    ) const = 0;
    // What is the probability that {team} gets {question} correct
    // when it is a {Direct/Pass}. Probability is scaled to [0, 100].
    virtual int getProbability(
        int team,
        int question,
        Route route
    ) const = 0;
    // After a question is done, who the next question should go to
    virtual int nextTeam(
        int directTeamOfPrevQuestion,
        int teamWhoAnsweredPrevQuestion
    ) const = 0;
};