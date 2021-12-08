/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Empiria Plain Object                    *
*  Type     : PolizaEntry                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for a policy.                                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Vouchers;

namespace Empiria.FinancialAccounting.Reporting.Domain {

  public interface IPolizaEntry {
  
  }

  /// <summary>Represents an entry for a policy.</summary>
  public class PolizaEntry : IPolizaEntry {


    #region Constructors and parsers

    internal PolizaEntry() {
      // Required by Empiria Framework.
    }

    #endregion Constructors and parsers

    [DataField("ID_MAYOR", ConvertFrom = typeof(decimal))]
    public Ledger Ledger {
      get;
      internal set;
    }

    [DataField("ID_TRANSACCION", ConvertFrom = typeof(decimal))]
    private decimal _id_transaccion = 0;

    private Voucher _voucher;

    public Voucher Voucher {
      get {
        if (_voucher != null) {
          return _voucher;
        } else {
          return Voucher.Parse((long) _id_transaccion);
        }
      }
      set {
        _voucher = value;
      }
    }


    [DataField("DEBE")]
    public decimal Debit {
      get;
      internal set;
    }


    [DataField("HABER")]
    public decimal Credit {
      get;
      internal set;
    }


    public int VouchersByLedger {
      get; internal set;
    }


    public ItemType ItemType {
      get; internal set;
    } = ItemType.Entry;



    internal void Sum(PolizaEntry voucher) {
      this.Debit += voucher.Debit;
      this.Credit += voucher.Credit;
      this.VouchersByLedger += 1; 
    }

  } // class PolizaEntry

  
} // namespace Empiria.FinancialAccounting.Reporting.Domain
