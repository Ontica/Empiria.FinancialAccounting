/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria General Object                  *
*  Type     : SubledgerType                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes the type of the accounts of a subledger (e.g. activo, clientes, proveedores, etc).   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting {

  /// <summary>Describes the type of the accounts of a subledger
  /// (e.g. activo, clientes, proveedores, etc).</summary>
  public class SubledgerType : GeneralObject {

    private SubledgerType() {
      // Required by Empiria Framework.
    }


    static public SubledgerType Parse(int id) {
      return BaseObject.ParseId<SubledgerType>(id);
    }


    static public SubledgerType Parse(string uid) {
      return BaseObject.ParseKey<SubledgerType>(uid);
    }


    static public SubledgerType Pending {
      get {
        const int PENDING_SUBLEDGER_TYPE_ID = 19;

        return SubledgerType.Parse(PENDING_SUBLEDGER_TYPE_ID);
      }
    }


    static public FixedList<SubledgerType> GetList() {
      return BaseObject.GetList<SubledgerType>()
                       .ToFixedList();
    }

    static public SubledgerType Empty => BaseObject.ParseEmpty<SubledgerType>();


  } // class SubledgerType

}  // namespace Empiria.FinancialAccounting
