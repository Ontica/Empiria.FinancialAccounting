/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Use case interactor class               *
*  Type     : BalanzaValorizadaRealUseCases              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to read Real valorize balance.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.DynamicData;

namespace Empiria.FinancialAccounting.Reclassification.Adapters {

  internal class BalanzaValorizadaRealMapper {

    #region Mappers

    static internal DynamicDto<BalanzaTradicionalRealDto> Map(FixedList<AccountReclassifiedBalances> entries) {
      return new DynamicDto<BalanzaTradicionalRealDto>(MapColumns(), MapEntries(entries));
    }


    static private FixedList<DataTableColumn> MapColumns() {
      return new DataTableColumn[] {
          new DataTableColumn("accountNo", "Cuenta", "text"),
          new DataTableColumn("accountName", "Nombre de la cuenta", "text"),
          new DataTableColumn("currencyCode", "Moneda", "text"),
          new DataTableColumn("initialBalance", "Saldo inicial", "decimal"),
          new DataTableColumn("debits", "Cargos", "decimal"),
          new DataTableColumn("credits", "Abonos", "decimal"),
          new DataTableColumn("finalBalance", "Saldo final", "decimal"),
          new DataTableColumn("realCurrencyCode", "Mon Real", "text"),
          new DataTableColumn("realInitialBalance", "Saldo inicial real", "decimal"),
          new DataTableColumn("realDebits", "Cargos real", "decimal"),
          new DataTableColumn("realCredits", "Abonos real", "decimal"),
          new DataTableColumn("realFinalBalance", "Saldo final real", "decimal")
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

  } // class BalanzaValorizadaRealMapper

} // namespace Empiria.FinancialAccounting.Reclassification.Adapters
