/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : ValorizacionEstimacionPreventivaMapper     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map valorized report.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Methods used to map valorized report.</summary>
  static internal class ValorizacionEstimacionPreventivaMapper {

    #region Public methods

    static internal ValorizacionEstimacionPreventivaDto Map(TrialBalanceQuery query,
                                        FixedList<ValorizacionEstimacionPreventivaEntry> entries) {
      if (entries.Count == 0) {
        return new ValorizacionEstimacionPreventivaDto();
      }
      var iEntries = new FixedList<ITrialBalanceEntry>(entries);

      return new ValorizacionEstimacionPreventivaDto {
        Query = query,
        Columns = DataColumns(iEntries),
        Entries = entries.Select(x => MapEntry(x))
                         .ToFixedList()
      };
    }


    public static FixedList<DataTableColumn> DataColumns(FixedList<ITrialBalanceEntry> entries) {


      var valorizations = entries.Select(x => (ValorizacionEstimacionPreventivaEntry) x);
      var columnsByCurrency = GetColumnsByCurrency(valorizations);

      List<DataTableColumn> columns = new List<DataTableColumn>();
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text-nowrap"));

      CurrenciesColumns(columns, columnsByCurrency);
      LastMonthColumns(columns, columnsByCurrency);
      CurrentMonthColumns(columns, columnsByCurrency);
      ValuedEffectColumns(columns, columnsByCurrency);

      columns.Add(new DataTableColumn("totalValued", "TOTAL", "decimal"));

      GetDynamicColumns(columns, valorizations);

      columns.Add(new DataTableColumn("totalAccumulated", "ACUMULADO", "decimal"));

      return columns.ToFixedList();
    }


    public static ValorizacionEntryDto MapEntry(ValorizacionEstimacionPreventivaEntry entry) {

      var dto = new ValorizacionEntryDto();

      dto.ItemType = entry.ItemType;
      dto.StandardAccountId = entry.Account.Id;
      dto.AccountName = entry.Account.Name;
      dto.AccountNumber = entry.Account.Number;
      dto.CurrencyCode = entry.Currency.Code;
      dto.ValuedExchangeRate = entry.ValuedExchangeRate;
      dto.LastChangeDate = entry.LastChangeDate;

      AssignValuesByCurrency(dto, entry);

      dynamic obj = dto;

      SetTotalsFields(obj, entry);

      return obj;
    }


    #endregion Public methods


    #region Private methods


    static private void AssignValuesByCurrency(ValorizacionEntryDto dto, ValorizacionEstimacionPreventivaEntry entry) {

      dto.USD = entry.ValuesByCurrency.USD;
      dto.EUR = entry.ValuesByCurrency.EUR;
      dto.YEN = entry.ValuesByCurrency.YEN;
      dto.UDI = entry.ValuesByCurrency.UDI;

      dto.LastUSD = entry.ValuesByCurrency.LastUSD;
      dto.LastYEN = entry.ValuesByCurrency.LastYEN;
      dto.LastEUR = entry.ValuesByCurrency.LastEUR;
      dto.LastUDI = entry.ValuesByCurrency.LastUDI;

      dto.CurrentUSD = entry.ValuesByCurrency.CurrentUSD;
      dto.CurrentYEN = entry.ValuesByCurrency.CurrentYEN;
      dto.CurrentEUR = entry.ValuesByCurrency.CurrentEUR;
      dto.CurrentUDI = entry.ValuesByCurrency.CurrentUDI;

      dto.ValuedEffectUSD = entry.ValuesByCurrency.ValuedEffectUSD;
      dto.ValuedEffectYEN = entry.ValuesByCurrency.ValuedEffectYEN;
      dto.ValuedEffectEUR = entry.ValuesByCurrency.ValuedEffectEUR;
      dto.ValuedEffectUDI = entry.ValuesByCurrency.ValuedEffectUDI;

      dto.TotalValued = entry.TotalValued;
      dto.TotalAccumulated = entry.TotalAccumulated;
    }


    private static void CurrenciesColumns(List<DataTableColumn> columns,
                                          ColumnsByCurrency columnsByCurrency) {
      if (columnsByCurrency.USDColumn) {
        columns.Add(new DataTableColumn("usd", "USD", "decimal"));
      }
      if (columnsByCurrency.YENColumn) {
        columns.Add(new DataTableColumn("yen", "YEN", "decimal"));
      }
      if (columnsByCurrency.EURColumn) {
        columns.Add(new DataTableColumn("eur", "EUR", "decimal"));
      }
      if (columnsByCurrency.UDIColumn) {
        columns.Add(new DataTableColumn("udi", "UDI", "decimal"));
      }
    }


    private static void CurrentMonthColumns(List<DataTableColumn> columns,
                                          ColumnsByCurrency columnsByCurrency) {
      if (columnsByCurrency.USDColumn) {
        columns.Add(new DataTableColumn("currentUSD", "Mes actual USD", "decimal"));
      }
      if (columnsByCurrency.YENColumn) {
        columns.Add(new DataTableColumn("currentYEN", "Mes actual YEN", "decimal"));
      }
      if (columnsByCurrency.EURColumn) {
        columns.Add(new DataTableColumn("currentEUR", "Mes actual EUR", "decimal"));
      }
      if (columnsByCurrency.UDIColumn) {
        columns.Add(new DataTableColumn("currentUDI", "Mes actual UDI", "decimal"));
      }
    }


    static private ColumnsByCurrency GetColumnsByCurrency(IEnumerable<ValorizacionEstimacionPreventivaEntry> valorizations) {

      var columns = new ColumnsByCurrency();

      columns.USDColumn = valorizations.Sum(a => a.ValuesByCurrency.USD) > 0 ? true : false;
      columns.YENColumn = valorizations.Sum(a => a.ValuesByCurrency.YEN) > 0 ? true : false;
      columns.EURColumn = valorizations.Sum(a => a.ValuesByCurrency.EUR) > 0 ? true : false;
      columns.UDIColumn = valorizations.Sum(a => a.ValuesByCurrency.UDI) > 0 ? true : false;

      return columns;
    }


    static private void GetDynamicColumns(List<DataTableColumn> columns,
                        IEnumerable<ValorizacionEstimacionPreventivaEntry> valorizations) {

      var entry = valorizations.FirstOrDefault();

      if (entry != null) {

        List<string> members = new List<string>();

        members.AddRange(entry.GetDynamicMemberNames());

        foreach (var member in members) {
          columns.Add(new DataTableColumn(member.ToLower(), member, "decimal"));
        }

      }

    }


    private static void LastMonthColumns(List<DataTableColumn> columns,
                                          ColumnsByCurrency columnsByCurrency) {
      if (columnsByCurrency.USDColumn) {
        columns.Add(new DataTableColumn("lastUSD", "Mes anterior USD", "decimal"));
      }
      if (columnsByCurrency.YENColumn) {
        columns.Add(new DataTableColumn("lastYEN", "Mes anterior YEN", "decimal"));
      }
      if (columnsByCurrency.EURColumn) {
        columns.Add(new DataTableColumn("lastEUR", "Mes anterior EUR", "decimal"));
      }
      if (columnsByCurrency.UDIColumn) {
        columns.Add(new DataTableColumn("lastUDI", "Mes anterior UDI", "decimal"));
      }
    }


    private static void ValuedEffectColumns(List<DataTableColumn> columns,
                                          ColumnsByCurrency columnsByCurrency) {
      if (columnsByCurrency.USDColumn) {
        columns.Add(new DataTableColumn("valuedEffectUSD", "Efecto valorización USD", "decimal"));
      }
      if (columnsByCurrency.YENColumn) {
        columns.Add(new DataTableColumn("valuedEffectYEN", "Efecto valorización YEN", "decimal"));
      }
      if (columnsByCurrency.EURColumn) {
        columns.Add(new DataTableColumn("valuedEffectEUR", "Efecto valorización EUR", "decimal"));
      }
      if (columnsByCurrency.UDIColumn) {
        columns.Add(new DataTableColumn("valuedEffectUDI", "Efecto valorización UDI", "decimal"));
      }
    }

    #endregion Private methods

    #region Helpers

    static private void SetTotalsFields(DynamicValorizacionEntryDto o, ValorizacionEstimacionPreventivaEntry entry) {
      var totalsColumns = entry.GetDynamicMemberNames();

      foreach (string column in totalsColumns) {
        o.SetTotalField(column, entry.GetTotalField(column));
      }
    }

    #endregion Helpers

  } // class ValorizacionEstimacionPreventivaMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
