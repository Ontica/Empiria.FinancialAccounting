/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Catalogues                                 Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Data Object                     *
*  Type     : Sector                                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds data about an ecomomic sector.                                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds data about an economic sector.</summary>
  public class Sector : BaseObject, INamedEntity {

    #region Constructors and parsers

    protected Sector() {
      // Required by Empiria Framework.
    }


    static public Sector Parse(int id) {
      return BaseObject.ParseId<Sector>(id);
    }


    static public Sector Parse(string uid) {
      return BaseObject.ParseKey<Sector>(uid);
    }


    public static bool Exists(string uid) {
      return BaseObject.TryParse<Sector>($"CLAVE_SECTOR = '{uid}'") != null;
    }


    static public FixedList<Sector> GetList() {
      return BaseObject.GetList<Sector>("ID_SECTOR > 0", "CLAVE_SECTOR")
                       .ToFixedList();
    }


    static public Sector Empty => BaseObject.ParseEmpty<Sector>();


    static public Sector Root {
      get {
        return Empty;
      }
    }

    #endregion Constructors and parsers

    #region Public properties


    [DataField("NOMBRE_SECTOR")]
    public string Name {
      get; private set;
    }


    [DataField("CLAVE_SECTOR")]
    public string Code {
      get; private set;
    }

    public string FullName {
      get {
        return $"({this.Code}) {this.Name}";
      }
    }

    [DataField("ID_SECTOR_PADRE")]
    private int _parentSectorId = -1;


    public Sector Parent {
      get {
        if (this.IsEmptyInstance) {
          return this;
        } else if (_parentSectorId == -1) {
          return Sector.Empty;
        } else {
          return Sector.Parse(_parentSectorId);
        }
      }
    }


    public bool HasParent {
      get {
        return !IsRoot;
      }
    }

    public bool HasChildren {
      get {
        return EmpiriaMath.IsMemberOf(this.Id, new []{-1, 4, 10});
      }
    }


    public bool IsRoot {
      get {
        return this.IsEmptyInstance;
      }
    }

    #endregion Public properties

    #region Methods

    public NamedEntityDto MapToNamedEntity() {
      return new NamedEntityDto(this.UID, this.FullName);
    }

    #endregion Methods

  }

}  // namespace Empiria.FinancialAccounting
