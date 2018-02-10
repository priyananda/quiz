#include "StrategyBuilder.h"

class StrategyImpl : public Strategy {
public:
    StrategyImpl(TeamDistribution dist, RouteBias bias, NegativesPolicy negPol, int pointsBase, RotationPolicy r) :
        dist_(dist), bias_(bias), negatives_(negPol), pointsBase_(pointsBase), rotation_(r)
    {
    }
    virtual int getPoints(
        int team,
        int question,
        bool isCorrect,
        Route route
    ) const override {
        return isCorrect ? pointsBase_ : 0;
    }
    virtual int getProbability(
        int team,
        int question,
        Route route
    ) const override {
        if (dist_ == TeamDistribution::Perfect)
            return true;
        return 50;
    }

    virtual int nextTeam(
        int directTeamOfPrevQuestion,
        int teamWhoAnsweredPrevQuestion
    ) const override {
        if (rotation_ == RotationPolicy::InfiniteBounce) {
            if (teamWhoAnsweredPrevQuestion < 0)
                return directTeamOfPrevQuestion;
            return teamWhoAnsweredPrevQuestion + 1;
        }
        return directTeamOfPrevQuestion + 1;
    }
private:
    TeamDistribution dist_;
    RouteBias bias_;
    NegativesPolicy negatives_;
    int pointsBase_;
    RotationPolicy rotation_;
};

std::unique_ptr<Strategy> StrategyBuilder::build() {
    std::unique_ptr<Strategy> ret {new StrategyImpl {
        dist_,
        bias_,
        negatives_,
        pointsBase_,
        rotation_
    }};
    return std::move(ret);
}