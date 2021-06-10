/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Catalogues Management                      Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Data Object                     *
*  Type     : FunctionalArea                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds data about a functional area.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds data about a functional area.</summary>
  public class FunctionalArea : Organization {

    #region Constructors and parsers

    private FunctionalArea() {
      // Required by Empiria Framework.
    }

    static public new FunctionalArea Parse(int id) {
      return BaseObject.ParseId<FunctionalArea>(id);
    }

    static public new FunctionalArea Parse(string uid) {
      return BaseObject.ParseKey<FunctionalArea>(uid);
    }

    static public FixedList<FunctionalArea> GetList() {
      return BaseObject.GetList<FunctionalArea>().ToFixedList();
    }

    static public new FunctionalArea Empty {
      get {
        return FunctionalArea.ParseEmpty<FunctionalArea>();
      }
    }

    #endregion Constructors and parsers

  } // class FunctionalArea

}  // namespace Empiria.FinancialAccounting
