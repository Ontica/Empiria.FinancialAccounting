using System;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Reporting.ValorizacionEstimacionPreventiva.Adaptars {

  static internal class ValorizacionPreventivaMapper {

    #region Public methods

    static internal ReportDataDto Map(ReportBuilderQuery query, TrialBalanceDto trialBalance) {

      var entries = trialBalance.Entries.Select(a => (ValorizacionEntryDto) a);

      return new ReportDataDto {
        Query = query,
        Columns = trialBalance.Columns,
        Entries = MapToReportDataEntries(entries.ToFixedList())
      };
    }


    private static FixedList<IReportEntryDto> MapToReportDataEntries(FixedList<ValorizacionEntryDto> entries) {

      var mappedItems = entries.Select((x) => MapEntry(x));

      return new FixedList<IReportEntryDto>(mappedItems);
    }


    public static ValorizacionPreventivaEntryDto MapEntry(ValorizacionEntryDto entry) {

      var dto = new ValorizacionPreventivaEntryDto();

      dto.ItemType = entry.ItemType;
      dto.StandardAccountId = entry.StandardAccountId;
      dto.AccountName = entry.AccountName;
      dto.AccountNumber = entry.AccountNumber;
      dto.CurrencyCode = entry.CurrencyCode;
      dto.ValuedExchangeRate = entry.ValuedExchangeRate;
      dto.LastChangeDate = entry.LastChangeDate;
      dto.MXN = entry.MXN;
      dto.MXNDebit = entry.MXNDebit;
      dto.MXNCredit = entry.MXNCredit;
      dto.DebtorCreditor = entry.DebtorCreditor.ToString();

      AssignValuesByCurrency(dto, entry);

      dynamic obj = dto;

      SetTotalsFields(obj, entry);

      return obj;
    }

    #endregion Public methods

    #region Private methods

    static private void AssignValuesByCurrency(ValorizacionPreventivaEntryDto dto, ValorizacionEntryDto entry) {

      dto.USD = entry.USD;
      dto.EUR = entry.EUR;
      dto.YEN = entry.YEN;
      dto.UDI = entry.UDI;

      dto.LastUSD = entry.LastUSD;
      dto.LastYEN = entry.LastYEN;
      dto.LastEUR = entry.LastEUR;
      dto.LastUDI = entry.LastUDI;

      dto.ValuedUSD = entry.ValuedUSD;
      dto.ValuedYEN = entry.ValuedYEN;
      dto.ValuedEUR = entry.ValuedEUR;
      dto.ValuedUDI = entry.ValuedUDI;

      dto.ValuedEffectUSD = entry.ValuedEffectUSD;
      dto.ValuedEffectYEN = entry.ValuedEffectYEN;
      dto.ValuedEffectEUR = entry.ValuedEffectEUR;
      dto.ValuedEffectUDI = entry.ValuedEffectUDI;

      dto.USDDebit = entry.USDDebit;
      dto.YENDebit = entry.YENDebit;
      dto.EURDebit = entry.EURDebit;
      dto.UDIDebit = entry.UDIDebit;

      dto.USDCredit = entry.USDCredit;
      dto.YENCredit = entry.YENCredit;
      dto.EURCredit = entry.EURCredit;
      dto.UDICredit = entry.UDICredit;

      dto.ValuedUSDDebit = entry.ValuedUSDDebit;
      dto.ValuedYENDebit = entry.ValuedYENDebit;
      dto.ValuedEURDebit = entry.ValuedEURDebit;
      dto.ValuedUDIDebit = entry.ValuedUDIDebit;

      dto.ValuedUSDCredit = entry.ValuedUSDCredit;
      dto.ValuedYENCredit = entry.ValuedYENCredit;
      dto.ValuedEURCredit = entry.ValuedEURCredit;
      dto.ValuedUDICredit = entry.ValuedUDICredit;

      dto.ValuedEffectDebitUSD = entry.ValuedEffectDebitUSD;
      dto.ValuedEffectDebitYEN = entry.ValuedEffectDebitYEN;
      dto.ValuedEffectDebitEUR = entry.ValuedEffectDebitEUR;
      dto.ValuedEffectDebitUDI = entry.ValuedEffectDebitUDI;

      dto.ValuedEffectCreditUSD = entry.ValuedEffectCreditUSD;
      dto.ValuedEffectCreditYEN = entry.ValuedEffectCreditYEN;
      dto.ValuedEffectCreditEUR = entry.ValuedEffectCreditEUR;
      dto.ValuedEffectCreditUDI = entry.ValuedEffectCreditUDI;

      dto.TotalValued = entry.TotalValued;
      dto.TotalAccumulated = entry.TotalAccumulated;
    }

    #endregion Private methods

    #region Helpers

    static private void SetTotalsFields(DynamicValorizacionEntryDto o, ValorizacionEntryDto entry) {
      var totalsColumns = entry.GetDynamicMemberNames();

      foreach (string column in totalsColumns) {
        o.SetTotalField(column, entry.GetTotalField(column));
      }
    }

    #endregion Helpers

  }

}
