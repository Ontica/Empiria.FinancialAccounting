/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Exchange Rates                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria General Object                  *
*  Type     : ExchangeRateType                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an exchange rate type.                                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting {

  /// <summary>Represents an exchange rate type.</summary>
  public class ExchangeRateType : GeneralObject {

    private ExchangeRateType() {
      // Required by Empiria Framework.
    }

    static public ExchangeRateType Parse(int id) {
      return BaseObject.ParseId<ExchangeRateType>(id);
    }


    static public FixedList<ExchangeRateType> GetList() {
      return BaseObject.GetList<ExchangeRateType>()
                       .ToFixedList();
    }

    static public ExchangeRateType Empty => BaseObject.ParseEmpty<ExchangeRateType>();

  } // class ExchangeRateType

}  // namespace Empiria.FinancialAccounting
