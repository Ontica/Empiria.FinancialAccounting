/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Adapters Layer                          *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Mapper                                  *
*  Type     : BalanzaAnaliticaOperacionesMapper          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Maps AccountReclassifiedBalances to a Dynamic<BalanzaAnaliticaOperacionesDto>.                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.DynamicData;

namespace Empiria.FinancialAccounting.Reclassification.Adapters {

  /// <summary>Maps AccountReclassifiedBalances to a Dynamic<BalanzaAnaliticaOperacionesDto>.</summary>
  internal class BalanzaAnaliticaOperacionesMapper {

    #region Mappers

    static internal DynamicDto<BalanzaAnaliticaOperacionesDto> Map(FixedList<AccountReclassifiedBalances> entries) {

      return new DynamicDto<BalanzaAnaliticaOperacionesDto>(MapColumns(), MapEntries(entries));
    }


    static private FixedList<DataTableColumn> MapColumns() {
      return new DataTableColumn[] {
          new DataTableColumn("operationType", "Tipo de transacción", "text"),
          new DataTableColumn("accountNo", "Cuenta", "text"),
          new DataTableColumn("accountName", "Nombre de la cuenta", "text"),
          new DataTableColumn("currencyCode", "Moneda", "text"),
          new DataTableColumn("debits", "Cargos", "decimal"),
          new DataTableColumn("credits", "Abonos", "decimal")
        }.ToFixedList();
    }


    static private FixedList<BalanzaAnaliticaOperacionesDto> MapEntries(FixedList<AccountReclassifiedBalances> entries) {
      return entries.Select(x => Map(x))
                    .ToFixedList();
    }


    static private BalanzaAnaliticaOperacionesDto Map(AccountReclassifiedBalances entry) {

      decimal total = entry.StdAccount.DebtorCreditor == DebtorCreditorType.Deudora ?
                              entry.RealDebits - entry.RealCredits :
                              entry.RealCredits - entry.RealDebits;

      return new BalanzaAnaliticaOperacionesDto {
        OperationType = entry.OperationType.Name,
        AccountNo = entry.StdAccount.Number,
        AccountName = entry.StdAccount.Name,
        CurrencyCode = entry.RealCurrency.ISOCode,
        Debits = entry.RealDebits,
        Credits = entry.RealCredits
      };
    }

    #endregion Mappers

  } // class BalanzaAnaliticaOperacionesMapper

} // namespace Empiria.FinancialAccounting.Reclassification.Adapters
