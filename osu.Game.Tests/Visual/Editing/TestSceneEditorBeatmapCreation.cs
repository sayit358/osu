// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Testing;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Osu;
using osu.Game.Screens.Edit.Setup;
using osu.Game.Tests.Resources;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;

namespace osu.Game.Tests.Visual.Editing
{
    public class TestSceneEditorBeatmapCreation : EditorTestScene
    {
        protected override Ruleset CreateEditorRuleset() => new OsuRuleset();

        protected override bool EditorComponentsReady => Editor.ChildrenOfType<SetupScreen>().SingleOrDefault()?.IsLoaded == true;

        public override void SetUpSteps()
        {
            AddStep("set dummy", () => Beatmap.Value = new DummyWorkingBeatmap(Audio, null));

            base.SetUpSteps();

            // if we save a beatmap with a hash collision, things fall over.
            // probably needs a more solid resolution in the future but this will do for now.
            AddStep("make new beatmap unique", () => EditorBeatmap.Metadata.Title = Guid.NewGuid().ToString());
        }

        [Test]
        public void TestCreateNewBeatmap()
        {
            AddStep("save beatmap", () => Editor.Save());
            AddAssert("new beatmap persisted", () => EditorBeatmap.BeatmapInfo.ID > 0);
        }

        [Test]
        public void TestAddAudioTrack()
        {
            AddAssert("switch track to real track", () =>
            {
                var setup = Editor.ChildrenOfType<SetupScreen>().First();

                var temp = TestResources.GetTestBeatmapForImport();

                string extractedFolder = $"{temp}_extracted";
                Directory.CreateDirectory(extractedFolder);

                using (var zip = ZipArchive.Open(temp))
                    zip.WriteToDirectory(extractedFolder);

                bool success = setup.ChangeAudioTrack(Path.Combine(extractedFolder, "03. Renatus - Soleily 192kbps.mp3"));

                File.Delete(temp);
                Directory.Delete(extractedFolder, true);

                return success;
            });

            AddAssert("track length changed", () => Beatmap.Value.Track.Length > 60000);
        }
    }
}