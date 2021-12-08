/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : VouchersByAccountEntry                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for vouchers by account.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Vouchers;

namespace Empiria.FinancialAccounting.BalanceEngine {

  public interface IVouchersByAccountEntry {

  }

  /// <summary>Represents an entry for vouchers by account.</summary>
  public class VouchersByAccountEntry : IVouchersByAccountEntry {

    [DataField("ID_MAYOR", ConvertFrom = typeof(decimal))]
    public Ledger Ledger {
      get;
      internal set;
    }

    [DataField("ID_MONEDA", ConvertFrom = typeof(decimal))]
    public Currency Currency {
      get;
      internal set;
    }


    [DataField("ID_CUENTA_ESTANDAR", ConvertFrom = typeof(long))]
    public StandardAccount Account {
      get;
      internal set;
    }


    [DataField("ID_SECTOR", ConvertFrom = typeof(long))]
    public Sector Sector {
      get;
      internal set;
    }



    [DataField("ID_CUENTA_AUXILIAR", ConvertFrom = typeof(decimal))]
    public int SubledgerAccountId {
      get;
      internal set;
    }


    [DataField("ID_TRANSACCION", ConvertFrom = typeof(decimal))]
    private decimal _id_transaccion = 0;

    public Voucher Voucher {
      get {
        return Voucher.Parse((long) _id_transaccion);
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



    public TrialBalanceItemType ItemType {
      get; internal set;
    }

  } // class VouchersByAccountEntry

} // namespace Empiria.FinancialAccounting.BalanceEngine
