/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Data Object                     *
*  Type     : Ledger                                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information about an accounting ledger book.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Contacts;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds information about an accounting ledger book.</summary>
  public class Ledger : BaseObject, INamedEntity {

    #region Constructors and parsers

    private Ledger() {
      // Required by Empiria Framework.
    }


    static public Ledger Parse(int id) {
      return BaseObject.ParseId<Ledger>(id);
    }

    static public Ledger Parse(string uid) {
      return BaseObject.ParseKey<Ledger>(uid);
    }

    static public Ledger Empty => BaseObject.ParseEmpty<Ledger>();


    #endregion Constructors and parsers

    #region Public properties

    [DataField("ID_TIPO_CUENTAS_STD", ConvertFrom=typeof(long))]
    internal AccountsChart AccountsChart {
      get; private set;
    }


    [DataField("NOMBRE_MAYOR")]
    public string Name {
      get; private set;
    } = string.Empty;


    [DataField("NUMERO_MAYOR")]
    public string Number {
      get; private set;
    } = string.Empty;


    [DataField("SUB_NUMERO_MAYOR")]
    public string Subnumber {
      get; private set;
    } = string.Empty;


    [DataField("PREFIJO_CUENTAS_AUXILIARES")]
    public string SubsidiaryAccountsPrefix {
      get; private set;
    } = string.Empty;


    [DataField("ID_MONEDA_BASE", ConvertFrom = typeof(long))]
    public Currency BaseCurrency {
      get; private set;
    }


    [DataField("ID_EMPRESA", ConvertFrom = typeof(long))]
    public Organization Organization {
      get; private set;
    }


    [DataField("ID_CALENDARIO", ConvertFrom = typeof(long))]
    public int CalendarId {
      get; private set;
    }


    [DataField("FECHA_CIERRE", Default = "ExecutionServer.DateMaxValue")]
    public DateTime EndDate {
      get; private set;
    }


    [DataField("ELIMINADO", ConvertFrom = typeof(int))]
    public bool Deleted {
      get; private set;
    }


    #endregion Public properties

  }  // class Ledger

}  // namespace Empiria.FinancialAccounting
