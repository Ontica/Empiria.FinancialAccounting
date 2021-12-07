/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Exchange Rates                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria General Object                  *
*  Type     : ExchangeRateType                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an exchange rate type.                                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting {

  /// <summary>Represents an exchange rate type.</summary>
  public class ExchangeRateType : GeneralObject {

    private ExchangeRateType() {
      // Required by Empiria Framework.
    }

    static public ExchangeRateType Parse(int id) {
      return BaseObject.ParseId<ExchangeRateType>(id);
    }


    static public ExchangeRateType Parse(string uid) {
      return BaseObject.ParseKey<ExchangeRateType>(uid);
    }


    static public FixedList<ExchangeRateType> GetList() {
      return BaseObject.GetList<ExchangeRateType>()
                       .ToFixedList();
    }

    static public ExchangeRateType Empty => BaseObject.ParseEmpty<ExchangeRateType>();


    static public ExchangeRateType ForVoucherEntries {
      get {
        return ExchangeRateType.Parse(46);
      }
    }


  } // class ExchangeRateType

}  // namespace Empiria.FinancialAccounting
