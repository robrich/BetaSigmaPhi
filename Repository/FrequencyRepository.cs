namespace BetaSigmaPhi.Repository {
	using System;
	using System.Linq;
	using BetaSigmaPhi.Entity;
	using System.Collections.Generic;

	public interface IFrequencyRepository {
		List<Frequency> GetAll();
	}

	public class FrequencyRepository : IFrequencyRepository {

		public List<Frequency> GetAll() {
			return Enum.GetValues(typeof(Frequency)).Cast<Frequency>().ToList();
		}

	}
}
