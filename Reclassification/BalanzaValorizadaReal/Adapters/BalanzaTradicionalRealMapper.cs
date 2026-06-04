/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Adapters Layer                          *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Mapper                                  *
*  Type     : BalanzaTradicionalRealMapper               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Maps AccountReclassifiedBalances to a Dynamic<BalanzaTradicionalRealDto>.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.DynamicData;

namespace Empiria.FinancialAccounting.Reclassification.Adapters {

  /// <summary>Maps AccountReclassifiedBalances to a Dynamic<BalanzaTradicionalRealDto>.</summary>
  internal class BalanzaTradicionalRealMapper {

    #region Mappers

    static internal DynamicDto<BalanzaTradicionalRealDto> Map(FixedList<AccountReclassifiedBalances> entries) {
      return new DynamicDto<BalanzaTradicionalRealDto>(MapColumns(), MapEntries(entries));
    }


    static private FixedList<DataTableColumn> MapColumns() {
      return new DataTableColumn[] {
          new DataTableColumn("accountNo", "Cuenta", "text"),
          new DataTableColumn("accountName", "Nombre de la cuenta", "text"),
          new DataTableColumn("operationType", "Tipo de transacción", "text"),
          new DataTableColumn("realCurrencyCode", "Mon", "text"),
          new DataTableColumn("realInitialBalance", "Saldo inicial", "decimal"),
          new DataTableColumn("realDebits", "Cargos", "decimal"),
          new DataTableColumn("realCredits", "Abonos", "decimal"),
          new DataTableColumn("realFinalBalance", "Saldo final", "decimal"),
          //new DataTableColumn("currencyCode", "Moneda (orig)", "text"),
          //new DataTableColumn("initialBalance", "Saldo inicial (orig)", "decimal"),
          //new DataTableColumn("debits", "Cargos (orig)", "decimal"),
          //new DataTableColumn("credits", "Abonos (orig)", "decimal"),
          //new DataTableColumn("finalBalance", "Saldo final (orig)", "decimal")
        }.ToFixedList();
    }


    static private FixedList<BalanzaTradicionalRealDto> MapEntries(FixedList<AccountReclassifiedBalances> entries) {
      return entries.Select(x => Map(x))
                    .ToFixedList();
    }


    static private BalanzaTradicionalRealDto Map(AccountReclassifiedBalances entry) {
      return new BalanzaTradicionalRealDto {
        AccountNo = entry.StdAccount.Number,
        AccountName = entry.StdAccount.Name,
        OperationType = entry.OperationType.IsEmptyInstance ? string.Empty : entry.OperationType.Name,
        CurrencyCode = entry.Currency.ISOCode,
        InitialBalance = entry.InitialBalance,
        Credits = entry.Credits,
        Debits = entry.Debits,
        FinalBalance = entry.FinalBalance,
        RealCurrencyCode = entry.RealCurrency.ISOCode,
        RealInitialBalance = entry.RealInitialBalance,
        RealCredits = entry.RealCredits,
        RealDebits = entry.RealDebits,
        RealFinalBalance = entry.RealFinalBalance
      };
    }

    #endregion Mappers

  } // class BalanzaTradicionalRealMapper

} // namespace Empiria.FinancialAccounting.Reclassification.Adapters
