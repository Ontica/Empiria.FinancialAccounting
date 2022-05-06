/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Catalogues                                 Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Mapper class                            *
*  Type     : CurrenciesMapper                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for currencies.                                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Mapping methods for currencies.</summary>
  static internal class CurrenciesMapper {


    static internal FixedList<CurrencyDto> MapCurrencies(FixedList<Currency> list) {
      return new FixedList<CurrencyDto>(list.Select((x) => MapCurrency(x)));
    }


    static internal CurrencyDto MapCurrency(Currency currency) {
      return new CurrencyDto {
        UID = currency.UID,
        Name = currency.Name,
        FullName = currency.FullName,
        Symbol = currency.Symbol,
        Code = currency.Code,
        Abbrev = currency.Abbrev
      };
    }

  }  // class CurrenciesMapper

}  // namespace Empiria.FinancialAccounting.Adapters
