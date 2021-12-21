using System;
using System.Collections.Generic;
using System.Linq;

namespace SoD_BaseMod.utils {
	public static class TableUtils {
		public class ColumnBuilder<T> {
			private readonly string headerLabel;
			private readonly Func<T, string> columnMapper;

			public ColumnBuilder(string headerLabel, Func<T, string> columnMapper) {
				this.headerLabel = headerLabel;
				this.columnMapper = columnMapper;
			}

			public string GetHeaderLabel() {
				return headerLabel;
			}

			public string GetColumnValue(T data) {
				return columnMapper.Invoke(data);
			}
		}

		public static IEnumerable<IEnumerable<string>> buildTable<T>(IEnumerable<T> dataList, params ColumnBuilder<T>[] columnBuilders) {
			return new[] { columnBuilders.Select(builder => builder.GetHeaderLabel()) }
					.Concat(dataList.Select(data => columnBuilders.Select(builder => builder.GetColumnValue(data))));
		}

		public static string tableToString(IEnumerable<IEnumerable<string>> table) {
			return string.Join("\n", table.Select(row => string.Join("\t", row)));
		}
	}
}