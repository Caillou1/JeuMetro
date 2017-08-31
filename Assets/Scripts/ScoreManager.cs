using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ScoreManager
{
    public static readonly int MaxScoreCount = 3;

	public class ScoreData
    {
        public ScoreData(int _score, bool _haveGoldAverageTime, bool _haveGoldMoneyLeft, bool _haveGoldSurface)
        {
            score = _score;
            haveGoldAverageTime = _haveGoldAverageTime;
            haveGoldMoneyLeft = _haveGoldMoneyLeft;
            haveGoldSurface = _haveGoldSurface;
        }

        public int score;
		public bool haveGoldAverageTime;
		public bool haveGoldMoneyLeft;
		public bool haveGoldSurface;
    }

    public class MedalData
    {
        public MedalData(bool _haveGoldAverageTime, bool _haveGoldMoneyLeft, bool _haveGoldSurface)
        {
			haveGoldAverageTime = _haveGoldAverageTime;
			haveGoldMoneyLeft = _haveGoldMoneyLeft;
			haveGoldSurface = _haveGoldSurface;
        }

		public bool haveGoldAverageTime;
		public bool haveGoldMoneyLeft;
		public bool haveGoldSurface;
    }

    public static void AddScore(ScoreData data, int level)
    {
        var scores = GetAllScore(level);

        bool inserted = false;
        for (int i = 0; i < scores.Count; i++)
        {
            if (scores[i].score < data.score)
            {
                scores.Insert(i, data);
                inserted = true;
                break;
            }
        }

        if (!inserted)
            scores.Add(data);

        for (int i = 0; i < Mathf.Min(scores.Count, MaxScoreCount); i++)
            WriteScores(scores[i], level, i);

		if (LevelLimit() < level)
			WriteLevelLimit(level);
	}

    public static ScoreData GetBestScore(int level)
    {
        return ReadScores(level, 0);
    }

    public static int ScoreCount(int level)
    {
        for (int i = 0; i < MaxScoreCount; i++)
            if (!PlayerPrefs.HasKey("Score" + level + "*" + i))
                return i;
        return MaxScoreCount;
    }

    public static int LevelLimit()
    {
        return ReadLevelLimit();
    }

    public static List<ScoreData> GetAllScore(int level)
    {
        List<ScoreData> scores = new List<ScoreData>();

        for (int i = 0; i < ScoreCount(level); i++)
            scores.Add(ReadScores(level, i));
        return scores;
    }

    public static bool IsLevelunlocked(int level)
    {
        return ReadLevelLimit() >= level;
    }

    public static void ClearAllScores()
    {
        PlayerPrefs.DeleteAll();
    }

    public static MedalData GetMedals(int level)
    {
        return ReadMedals(level);
    }

    static void WriteScores(ScoreData data, int level, int index)
    {
        PlayerPrefs.SetInt("Score" + level + "*" + index, data.score);
        PlayerPrefs.SetInt("AverageTime" + level + "*" + index, data.haveGoldAverageTime ? 1 : 0);
        PlayerPrefs.SetInt("Money" + level + "*" + index, data.haveGoldMoneyLeft ? 1 : 0);
        PlayerPrefs.SetInt("Surface" + level + "*" + index, data.haveGoldSurface ? 1 : 0);
    }

    static void WriteMedals(MedalData data, int level)
    {
		PlayerPrefs.SetInt("AverageTime" + level, data.haveGoldAverageTime ? 1 : 0);
		PlayerPrefs.SetInt("Money" + level, data.haveGoldMoneyLeft ? 1 : 0);
		PlayerPrefs.SetInt("Surface" + level, data.haveGoldSurface ? 1 : 0);
	}

	static void WriteLevelLimit(int level)
	{
        PlayerPrefs.SetInt("LevelLimit", level);
	}

    static ScoreData ReadScores(int level, int index)
    {
        return new ScoreData(
            PlayerPrefs.GetInt("Score" + level + "*" + index, 0),
    		PlayerPrefs.GetInt("AverageTime" + level + "*" + index, 0) != 0,
    		PlayerPrefs.GetInt("Money" + level + "*" + index, 0) != 0,
    		PlayerPrefs.GetInt("Surface" + level + "*" + index, 0) != 0);
    }

    static MedalData ReadMedals(int level)
    {
        return new MedalData(
		PlayerPrefs.GetInt("AverageTime" + level, 0) != 0,
			PlayerPrefs.GetInt("Money" + level, 0) != 0,
			PlayerPrefs.GetInt("Surface" + level, 0) != 0);
    }

    static int ReadLevelLimit()
    {
        return PlayerPrefs.GetInt("LevelLimit", 1);
    }
}
