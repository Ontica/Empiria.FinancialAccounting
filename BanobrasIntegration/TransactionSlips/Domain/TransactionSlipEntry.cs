/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Transaction Slips                             Component : Domain types                         *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Information holder                   *
*  Type     : TransactionSlipEntry                          License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Holds data about a transaction slip entry (movimiento en volante).                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips {


  /// <summary>Holds data about a transaction slip entry (movimiento en volante).</summary>
  internal class TransactionSlipEntry {

    #region Properties

    public string UID {
      get {
        return this.EntryNumber.ToString();
      }
    }


    [DataField("MCO_FOLIO", ConvertFrom = typeof(long))]
    public int EntryNumber {
      get;
      private set;
    }


    [DataField("MCO_AREA")]
    public string FunctionalArea {
      get;
      private set;
    }


    [DataField("MCO_REG_CONTABLE")]
    public string AccountNumber {
      get;
      private set;
    }


    [DataField("MCO_SECTOR")]
    public string SectorCode {
      get;
      private set;
    }


    [DataField("MCO_NUM_AUX")]
    public string SubledgerAccount {
      get;
      private set;
    }


    [DataField("MCO_MONEDA", ConvertFrom = typeof(long))]
    public int CurrencyCode {
      get;
      private set;
    }


    [DataField("MCO_T_CAMBIO")]
    public decimal ExchangeRate {
      get;
      private set;
    }


    public decimal Debit {
      get {
        return DebitCreditFlag == 1 ? Total : 0;
      }
    }


    public decimal Credit {
      get {
        return DebitCreditFlag == 2 ? Total : 0;
      }
    }


    [DataField("MCO_IMPORTE")]
    private decimal Total {
      get;
      set;
    }


    [DataField("MCO_CVE_MOV", ConvertFrom = typeof(long))]
    private int DebitCreditFlag {
      get;
      set;
    }


    [DataField("MCO_DESCRIP")]
    public string Description {
      get;
      private set;
    }

    #endregion Properties

  }  // class TransactionSlipEntry

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips
