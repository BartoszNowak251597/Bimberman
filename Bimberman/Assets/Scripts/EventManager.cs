using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager {
	public delegate void PotionExplodeEventReceiver(PotionExplodeEvent e);
	public delegate void TargetHitEventReceiver(TargetHitEvent e);

	private static event PotionExplodeEventReceiver potionExplodeEvents = e => {};
	private static event TargetHitEventReceiver targetHitEvents = e => {};

	public static void Subscribe(PotionExplodeEventReceiver r) {
		if (r != null) {
			potionExplodeEvents += r;
		}
	}
	public static void Subscribe(TargetHitEventReceiver r) {
		if (r != null) {
			targetHitEvents += r;
		}
	}

	public static void Unsubscribe(PotionExplodeEventReceiver r) {
		if (r != null) {
			potionExplodeEvents -= r;
		}
	}
	public static void Unsubscribe(TargetHitEventReceiver r) {
		if (r != null) {
			targetHitEvents -= r;
		}
	}

	public static void Emit(PotionExplodeEvent e) {
		potionExplodeEvents(e);
	}
	public static void Emit(TargetHitEvent e) {
		targetHitEvents(e);
	}
}

public struct PotionExplodeEvent {
	public Vector3 position;
	public float radius;
	public string name;
	public float effectTime;
}

public struct TargetHitEvent {
	public Skeleton target;
}