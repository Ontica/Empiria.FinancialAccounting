/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Empiria General Object                  *
*  Type     : TransactionType                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a business transaction type related with an accounting voucher.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Describes a business transaction type related with an accounting voucher.</summary>
  public class TransactionType : GeneralObject {

    private TransactionType() {
      // Required by Empiria Framework.
    }

    static public TransactionType Parse(int id) {
      return BaseObject.ParseId<TransactionType>(id);
    }


    static public TransactionType Parse(string uid) {
      return BaseObject.ParseKey<TransactionType>(uid);
    }


    static public FixedList<TransactionType> GetList() {
      return BaseObject.GetList<TransactionType>(string.Empty, "ObjectName")
                       .ToFixedList();
    }

    static public TransactionType Empty => BaseObject.ParseEmpty<TransactionType>();

  } // class TransactionType

}  // namespace Empiria.FinancialAccounting.Vouchers
