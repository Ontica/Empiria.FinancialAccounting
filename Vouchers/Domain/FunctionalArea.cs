/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Empiria Data Object                     *
*  Type     : FunctionalArea                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds data about a functional area.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Holds data about a functional area.</summary>
  public class FunctionalArea : BaseObject {

    #region Constructors and parsers

    protected FunctionalArea() {
      // Required by Empiria Framework.
    }


    static public FunctionalArea Parse(int id) {
      return BaseObject.ParseId<FunctionalArea>(id);
    }


    static public FunctionalArea Parse(string uid) {
      return BaseObject.ParseKey<FunctionalArea>(uid);
    }


    static public FunctionalArea TryParse(string areaID) {
      return BaseObject.TryParse<FunctionalArea>($"ParticipantKey = '{areaID}'");
    }


    static public FixedList<FunctionalArea> GetList() {
      string filter = "ParticipantType = 'O' AND Status = 'A'";
      string orderBy = "ParticipantKey";

      return BaseObject.GetList<FunctionalArea>(filter, orderBy)
                       .ToFixedList();
    }


    static public FunctionalArea Empty {
      get {
        return FunctionalArea.ParseEmpty<FunctionalArea>();
      }
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ParticipantName")]
    public string Name {
      get; private set;
    }

    [DataField("ParticipantKey")]
    public string Code {
      get; private set;
    }

    public string FullName {
      get {
        return $"({Code}) {Name}";
      }
    }

    #endregion Properties

    public NamedEntityDto MapToNamedEntity() {
      return new NamedEntityDto(this.Id.ToString(), this.FullName);
    }

  } // class FunctionalArea

}  // namespace Empiria.FinancialAccounting.Vouchers
