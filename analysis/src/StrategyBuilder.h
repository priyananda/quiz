#pragma once
#include <memory>
#include "Strategy.h"

// How strong the teams are, relative to one another
enum class TeamDistribution {
    Uniform, // All teams are similarly strong
    Normal, // A few teams are strong, and few aren't
    Perfect // Every team gets everything correct
};

enum class RouteBias {
    None,   // Direct and pass get the same points
    Direct, // Direct gets more
    Pass    // Pass gets more
};

enum class NegativesPolicy {
    None, // No Negatives
    OnPar, // wrong answer = -(points for correct answer)
    Half, // wrong answer = -0.5 * (points for correct answer)
};

enum class RotationPolicy {
    InfiniteBounce,
    RoundRobin
};

class StrategyBuilder {
public:

    StrategyBuilder& setTeamDistribution(TeamDistribution td) {
        dist_ = td;
        return *this;
    }

    StrategyBuilder& setRouteBias(RouteBias b) {
        bias_ = b;
        return *this;
    }

    StrategyBuilder& setNegativesPolicy(NegativesPolicy p) {
        negatives_ = p;
        return *this;
    }

    StrategyBuilder& setPoints(int p) {
        pointsBase_ = p;
        return *this;
    }

    StrategyBuilder& setRotationPolicy(RotationPolicy p) {
        rotation_ = p;
        return *this;
    }

    std::unique_ptr<Strategy> build();

private:
    TeamDistribution dist_ = TeamDistribution::Uniform;
    RouteBias bias_ = RouteBias::None;
    NegativesPolicy negatives_ = NegativesPolicy::None;
    int pointsBase_ = 10;
    RotationPolicy rotation_ = RotationPolicy::InfiniteBounce;
};