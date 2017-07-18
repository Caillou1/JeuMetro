using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScaleAnimator : MonoBehaviour {
	public float AnimationTime;
	public float ScaleFactor;
	public Ease EaseType;

	private Vector3 OriginalScale;
	private Vector3 TargetScale;

	private RectTransform rtf;

	void Start () {
		rtf = GetComponent<RectTransform> ();

		OriginalScale = rtf.localScale;
		TargetScale = OriginalScale * ScaleFactor;

		PlayAnimation ();
	}

	void PlayAnimation() {
		rtf.DOScale (TargetScale, AnimationTime / 2f).SetEase(EaseType).OnComplete(() => {
			rtf.DOScale(OriginalScale, AnimationTime / 2f).SetEase(EaseType).OnComplete(() => {
				PlayAnimation();
			});
		});
	}
}
