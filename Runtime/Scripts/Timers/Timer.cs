using UnityEngine;
using System;
using HereticalSolutions.Messaging;
using HereticalSolutions.Collections;

namespace HereticalSolutions.Timers
{
	public class Timer
	{
		public float Countdown { get; private set; }

		public float DefaultDuration { get; private set; }

		public Action Callback { get; set; }

		private PingerSubscription subscription;

		private IPoolElement<PingerSubscription> subscriptionElement;

		private Pinger pinger;

		public Timer(
			float defaultDuration,
			Pinger pinger)
		{
			Countdown = -1f;

			DefaultDuration = defaultDuration;

			this.pinger = pinger;

			subscription = new PingerSubscription(Ping);
		}

		public void Start()
		{
			Countdown = DefaultDuration;

			subscriptionElement = pinger.Subscribe(subscription);
		}

		public void Start(
			float duration)
		{
			Countdown = duration;

			subscriptionElement = pinger.Subscribe(subscription);
		}

		private void Ping()
		{
			Countdown -= Time.deltaTime;

			if (Countdown < 0f)
			{
				TimerExpired();
			}
		}

		private void TimerExpired()
		{
			Countdown = -1f;

			pinger.Unsubscribe(subscriptionElement);

			subscriptionElement = null;

			Callback?.Invoke();
		}
	}
}