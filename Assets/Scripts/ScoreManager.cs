using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ScoreManager
{
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

    /*static void AddScore(ScoreData data, int level)
    {
        
    }

    static ScoreData GetBestScore(int level)
    {
        
    }

    static int ScoreCount(int level)
    {
        
    }

    static int ScoreLevelsCount()
    {
        
    }

    static List<ScoreData> GetAllScore(int level)
    {
        
    }

    static bool isLevelunlocked(int level)
    {
        
    }

    static void ClearAllScores()
    {
        
    }*/
}
