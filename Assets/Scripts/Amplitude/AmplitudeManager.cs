using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AmplitudeManager
{
	public string ApiKey = "5df5e4c504c6f123761f6348e28427db";
	private Amplitude _amplitude;
	private SubscriberList _subscriberList = new SubscriberList();

	public AmplitudeManager()
	{
		_subscriberList.Subscribe();

        _amplitude = Amplitude.instance;
        _amplitude.apikey = ApiKey;

        OnInitialize();
	}

    void OnInitialize()
    {
        var data = new InitializeData();
        data.TotalyLegitRandomNumber = 4;
        data.Plateform = Application.platform.ToString();
        _amplitude.SendEvent("InitializeTest", data);
    }

    [Serializable]
    class InitializeData
    {
        public int TotalyLegitRandomNumber;
        public String Plateform;
    }
}

