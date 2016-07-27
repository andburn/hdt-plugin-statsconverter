using System;
using System.Collections.Generic;
using System.Linq;
using HDT.Plugins.Common.Models;
using HDT.Plugins.Common.Util;

namespace HDT.Plugins.StatsConverter.Utilities
{
	public class StatsFilter
	{
		public Guid? Deck { get; private set; }
		public Region Region { get; private set; }
		public GameMode Mode { get; private set; }
		public TimeFrame TimeFrame { get; private set; }

		public StatsFilter()
		{
			Deck = null;
			Region = Region.ALL;
			Mode = GameMode.ALL;
			TimeFrame = TimeFrame.ALL;
		}

		public StatsFilter(Guid? deck, Region region, GameMode mode, TimeFrame time)
		{
			Deck = deck;
			Region = region;
			Mode = mode;
			TimeFrame = time;
		}

		public List<GameStats> Apply(List<DeckStats> stats)
		{
			// filter by deck first if needed
			IEnumerable<GameStats> filtered = null;
			if (Deck != null)
			{
				filtered = stats.Where(d => d.DeckId.Equals(Deck)).SelectMany(d => d.Games);
			}
			else
			{
				filtered = stats.SelectMany(d => d.Games).ToList();
			}
			// region filter
			if (!Region.Equals(Region.ALL))
			{
				filtered = filtered.Where(g => (int)g.Region == (int)Region);
			}
			// game mode filter
			if (!Mode.Equals(GameMode.ALL))
			{
				filtered = filtered.Where(g => g.GameMode.Equals(Mode));
			}
			// time filter
			var times = GetFilterTimes(TimeFrame);
			filtered = filtered.Where(g => g.StartTime >= times.Item1 && g.EndTime <= times.Item2);

			// finally sort by time
			return filtered.OrderByDescending(g => g.EndTime).ToList();
		}

		private Tuple<DateTime, DateTime> GetFilterTimes(TimeFrame tf)
		{
			var startTime = DateTime.Today;
			var endTime = DateTime.Today + new TimeSpan(0, 23, 59, 59, 999);

			switch (TimeFrame)
			{
				case TimeFrame.TODAY:
					endTime = DateTime.Now;
					break;

				case TimeFrame.YESTERDAY:
					startTime -= new TimeSpan(1, 0, 0, 0);
					endTime -= new TimeSpan(1, 0, 0, 0);
					break;

				case TimeFrame.LAST_24_HOURS:
					startTime = DateTime.Now - new TimeSpan(1, 0, 0, 0);
					endTime = DateTime.Now;
					break;

				case TimeFrame.THIS_WEEK:
					startTime -= new TimeSpan(((int)(startTime.DayOfWeek) - 1), 0, 0, 0);
					break;

				case TimeFrame.PREVIOUS_WEEK:
					startTime -= new TimeSpan(7 + ((int)(startTime.DayOfWeek) - 1), 0, 0, 0);
					endTime -= new TimeSpan(((int)(endTime.DayOfWeek)), 0, 0, 0);
					break;

				case TimeFrame.LAST_7_DAYS:
					startTime -= new TimeSpan(7, 0, 0, 0);
					break;

				case TimeFrame.THIS_MONTH:
					startTime -= new TimeSpan(startTime.Day - 1, 0, 0, 0);
					break;

				case TimeFrame.PREVIOUS_MONTH:
					startTime -= new TimeSpan(startTime.Day - 1 + DateTime.DaysInMonth(startTime.AddMonths(-1).Year, startTime.AddMonths(-1).Month), 0, 0, 0);
					endTime -= new TimeSpan(endTime.Day, 0, 0, 0);
					break;

				case TimeFrame.THIS_YEAR:
					startTime -= new TimeSpan(startTime.DayOfYear - 1, 0, 0, 0);
					break;

				case TimeFrame.PREVIOUS_YEAR:
					startTime -= new TimeSpan(startTime.DayOfYear - 1 + (DateTime.IsLeapYear(startTime.Year) ? 366 : 365), 0, 0, 0);
					endTime -= new TimeSpan(startTime.DayOfYear, 0, 0, 0);
					break;

				default:
					startTime = new DateTime();
					break;
			}

			return new Tuple<DateTime, DateTime>(startTime, endTime);
		}
	}
}