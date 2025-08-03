﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Catalogues                                 Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Data Object                     *
*  Type     : Currency                                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds data about a currency.                                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds data about a currency.</summary>
  public class Currency : BaseObject, INamedEntity {

    #region Constructors and parsers

    protected Currency() {
      // Required by Empiria Framework.
    }


    static public Currency Parse(int id) {
      return BaseObject.ParseId<Currency>(id);
    }


    static public Currency Parse(string code) {
      return BaseObject.ParseKey<Currency>(code);
    }


    static public Currency TryParse(string code) {
      return BaseObject.TryParse<Currency>($"O_ID_MONEDA = '{code}'");
    }


    static public bool Exists(string code) {
      return (TryParse(code) != null);
    }


    static public Currency TryParseISOCode(string isoCode) {
      switch (isoCode.ToUpperInvariant()) {
        case "MXN":
          return Currency.MXN;
        case "USD":
          return Currency.USD;
        case "JPY":
          return Currency.YEN;
        case "MXV":
        case "UDI":
          return Currency.UDI;
        case "EUR":
          return Currency.EUR;
        default:
          return null;
      }
    }


    static public FixedList<Currency> GetList() {
      return BaseObject.GetList<Currency>("CURRENCY_ID <> -1", "O_ID_MONEDA")
                       .ToFixedList();
    }


    static public Currency Empty => BaseObject.ParseEmpty<Currency>();

    static public Currency MXN => Parse("01");

    static public Currency USD => Parse("02");

    static public Currency YEN => Parse("06");

    static public Currency EUR => Parse("27");

    static public Currency UDI => Parse("44");

    #endregion Constructors and parsers

    #region Public properties


    public string ISOCode {
      get {
        if (this.Equals(Currency.MXN)) {
          return "MXN";
        }
        if (this.Equals(Currency.USD)) {
          return "USD";
        }
        if (this.Equals(Currency.YEN)) {
          return "JPY";
        }
        if (this.Equals(Currency.EUR)) {
          return "EUR";
        }
        if (this.Equals(Currency.UDI)) {
          return "UDI";
        }
        return string.Empty;
      }
    }


    [DataField("CURRENCY_NAME")]
    public string Name {
      get; private set;
    }


    public string ShortName {
      get {
        if (this.Equals(Currency.MXN)) {
          return "M.N.";
        }
        if (this.Equals(Currency.USD)) {
          return "Dlls";
        }
        if (this.Equals(Currency.YEN)) {
          return "Yenes";
        }
        if (this.Equals(Currency.EUR)) {
          return "Euros";
        }
        if (this.Equals(Currency.UDI)) {
          return "UDI";
        }
        return this.Name;
      }
    }


    [DataField("ABBREV")]
    public string Abbrev {
      get; private set;
    }


    [DataField("O_ID_MONEDA")]
    public string Code {
      get; private set;
    }


    [DataField("SYMBOL")]
    public string Symbol {
      get; private set;
    }


    public string FullName {
      get {
        return $"({this.Code}) {this.Name}";
      }
    }

    #endregion Public properties

    #region Methods

    public NamedEntityDto MapToNamedEntity() {
      return new NamedEntityDto(this.UID, this.FullName);
    }

    #endregion Methods

  }  // class Currency

}  // namespace Empiria.FinancialAccounting
