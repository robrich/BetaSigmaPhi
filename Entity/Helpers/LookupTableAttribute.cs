namespace BetaSigmaPhi.Entity.Helpers {
	using System;

	public class LookupTableAttribute : Attribute {
		public string Table { get; set; }
		public string TextColumn { get; set; }
		public string ValueColumn { get; set; }

		public LookupTableAttribute( string Table, string TextColumn, string ValueColumn ) {
			if ( Table == null ) {
				throw new ArgumentNullException( "Table" );
			}
			if ( TextColumn == null ) {
				throw new ArgumentNullException( "TextColumn" );
			}
			if ( ValueColumn == null ) {
				throw new ArgumentNullException( "ValueColumn" );
			}
			this.Table = Table;
			this.TextColumn = TextColumn;
			this.ValueColumn = ValueColumn;
		}

		public override string ToString() {
			return string.Format(
				"SELECT {0} + ' = ' + CAST( {1} AS nvarchar(20)) + ',' FROM {2}",
				this.TextColumn, this.ValueColumn, this.Table
			);
		}

		public override int GetHashCode() {
			return this.ToString().GetHashCode();
		}

	}
}
