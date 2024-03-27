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

    internal readonly int MIN_EDITION_DAYS = ConfigurationData.Get("ExchangeRateEdition.MinDays", 20);
    internal readonly int MAX_EDITION_DAYS = ConfigurationData.Get("ExchangeRateEdition.MaxDays", 4);

    #region Constructors and parsers

    protected ExchangeRateType() {
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

    static public ExchangeRateType Diario => ExchangeRateType.Parse(46);

    static public ExchangeRateType Dolarizacion => ExchangeRateType.Parse(120);

    static public ExchangeRateType ValorizacionBanxico => ExchangeRateType.Parse(49);

    #endregion Constructors and parsers

    #region Properties

    public FixedList<Currency> Currencies {
      get {
        return base.ExtendedDataField.GetFixedList<Currency>("currencies", false);
      }
    }

    #endregion Properties

    #region Methods

    internal bool EditionValidOn(DateTime date) {
      if (DateTime.Today.AddDays(-1 * MIN_EDITION_DAYS) <= date &&
          date <= DateTime.Today.AddDays(MAX_EDITION_DAYS)) {
        return true;
      }
      return false;
    }


    internal bool HasCurrency(Currency currency) {
      Assertion.Require(currency, nameof(currency));

      return !currency.IsEmptyInstance;
    }

    #endregion Methods

  } // class ExchangeRateType

}  // namespace Empiria.FinancialAccounting
