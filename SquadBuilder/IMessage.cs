using System;
namespace SquadBuilder {
	public interface IMessage {
		void LongAlert (string message);
		void ShortAlert (string message);
	}
}
