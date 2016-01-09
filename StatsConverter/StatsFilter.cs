using System;
using System.Collections.Generic;
using System.Linq;

namespace AndBurn.HDT.Plugins.StatsConverter
{
	public class StatsFilter
	{
		public Guid? Deck { get; private set; }
		public StatsRegion Region { get; private set; }
		public GameMode Mode { get; private set; }
		public TimeFrame TimeFrame { get; private set; }

		public StatsFilter()
		{
			Deck = null;
			Region = StatsRegion.All;
			Mode = GameMode.All;
			TimeFrame = TimeFrame.AllTime;
		}

		public StatsFilter(Guid? deck, StatsRegion region, GameMode mode, TimeFrame time)
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
				filtered = stats.Where<DeckStats>(d => d.DeckId.Equals(Deck)).SelectMany(d => d.Games);
			}
			else
			{
				filtered = stats.SelectMany(d => d.Games).ToList<GameStats>();
			}
			// region filter
			if (!Region.Equals(StatsRegion.All))
			{
				filtered = filtered.Where<GameStats>(g => (int)g.Region == (int)Region);
			}
			// game mode filter
			if (!Mode.Equals(GameMode.All))
			{
				filtered = filtered.Where<GameStats>(g => g.GameMode.Equals(Mode));
			}
			// time filter
			var times = GetFilterTimes(TimeFrame);
			filtered = filtered.Where<GameStats>(g => g.StartTime >= times.Item1 && g.EndTime <= times.Item2);

			// finally sort by time
			return filtered.OrderByDescending<GameStats, DateTime>(g => g.EndTime).ToList<GameStats>();
		}

		private Tuple<DateTime, DateTime> GetFilterTimes(TimeFrame tf)
		{
			var startTime = DateTime.Today;
			var endTime = DateTime.Today + new TimeSpan(0, 23, 59, 59, 999);

			switch (TimeFrame)
			{
				case TimeFrame.Today:
					endTime = DateTime.Now;
					break;

				case TimeFrame.Yesterday:
					startTime -= new TimeSpan(1, 0, 0, 0);
					endTime -= new TimeSpan(1, 0, 0, 0);
					break;

				case TimeFrame.Last24Hours:
					startTime = DateTime.Now - new TimeSpan(1, 0, 0, 0);
					endTime = DateTime.Now;
					break;

				case TimeFrame.ThisWeek:
					startTime -= new TimeSpan(((int)(startTime.DayOfWeek) - 1), 0, 0, 0);
					break;

				case TimeFrame.PreviousWeek:
					startTime -= new TimeSpan(7 + ((int)(startTime.DayOfWeek) - 1), 0, 0, 0);
					endTime -= new TimeSpan(((int)(endTime.DayOfWeek)), 0, 0, 0);
					break;

				case TimeFrame.Last7Days:
					startTime -= new TimeSpan(7, 0, 0, 0);
					break;

				case TimeFrame.ThisMonth:
					startTime -= new TimeSpan(startTime.Day - 1, 0, 0, 0);
					break;

				case TimeFrame.PreviousMonth:
					startTime -= new TimeSpan(startTime.Day - 1 + DateTime.DaysInMonth(startTime.AddMonths(-1).Year, startTime.AddMonths(-1).Month), 0, 0, 0);
					endTime -= new TimeSpan(endTime.Day, 0, 0, 0);
					break;

				case TimeFrame.ThisYear:
					startTime -= new TimeSpan(startTime.DayOfYear - 1, 0, 0, 0);
					break;

				case TimeFrame.PreviousYear:
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