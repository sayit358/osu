﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays;
using osu.Game.Overlays.Rankings;
using osu.Game.Rulesets;
using osu.Game.Users;

namespace osu.Game.Tests.Visual.Online
{
    public class TestSceneRankingsHeader : OsuTestScene
    {
        public override IReadOnlyList<Type> RequiredTypes => new[]
        {
            typeof(RankingsOverlayHeader),
            typeof(CountryFilter),
            typeof(CountryPill)
        };

        [Cached]
        private readonly OverlayColourProvider overlayColour = new OverlayColourProvider(OverlayColourScheme.Green);

        public TestSceneRankingsHeader()
        {
            var countryBindable = new Bindable<Country>();
            var ruleset = new Bindable<RulesetInfo>();
            var scope = new Bindable<RankingsScope>();

            Add(new RankingsOverlayHeader
            {
                Current = { BindTarget = scope },
                Country = { BindTarget = countryBindable },
                Ruleset = { BindTarget = ruleset },
                Spotlights = new[]
                {
                    new APISpotlight
                    {
                        Id = 1,
                        Name = "Spotlight 1"
                    },
                    new APISpotlight
                    {
                        Id = 2,
                        Name = "Spotlight 2"
                    },
                    new APISpotlight
                    {
                        Id = 3,
                        Name = "Spotlight 3"
                    }
                }
            });

            var country = new Country
            {
                FlagName = "BY",
                FullName = "Belarus"
            };

            var unknownCountry = new Country
            {
                FlagName = "CK",
                FullName = "Cook Islands"
            };

            AddStep("Set country", () => countryBindable.Value = country);
            AddStep("Set scope to Score", () => scope.Value = RankingsScope.Score);
            AddStep("Set country with no flag", () => countryBindable.Value = unknownCountry);
        }
    }
}
