﻿using System.IO;

namespace PuzzleEngineAlpha.Databases.Level
{
    using PuzzleEngineAlpha.Level;

    public interface ILevelInfoDB
    {
        LevelInfo Load(FileStream fileStream);

        void Save(FileStream fileStream, LevelInfo levelInfo);
    }
}
