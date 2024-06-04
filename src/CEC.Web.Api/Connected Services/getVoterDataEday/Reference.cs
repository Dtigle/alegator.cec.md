﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CEC.Web.Api.getVoterDataEday {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="VoterData", Namespace="http://schemas.datacontract.org/2004/07/CEC.SAISE.EDayModule.WebServices.Modeldto" +
        "")]
    [System.SerializableAttribute()]
    public partial class VoterData : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string VoterNumberListField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string VoterCertificatatNumberField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PollingStationNumberField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CircumscriptionNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CircumscriptionNumberField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string VoterNumberList {
            get {
                return this.VoterNumberListField;
            }
            set {
                if ((object.ReferenceEquals(this.VoterNumberListField, value) != true)) {
                    this.VoterNumberListField = value;
                    this.RaisePropertyChanged("VoterNumberList");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=1)]
        public string VoterCertificatatNumber {
            get {
                return this.VoterCertificatatNumberField;
            }
            set {
                if ((object.ReferenceEquals(this.VoterCertificatatNumberField, value) != true)) {
                    this.VoterCertificatatNumberField = value;
                    this.RaisePropertyChanged("VoterCertificatatNumber");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=2)]
        public string PollingStationNumber {
            get {
                return this.PollingStationNumberField;
            }
            set {
                if ((object.ReferenceEquals(this.PollingStationNumberField, value) != true)) {
                    this.PollingStationNumberField = value;
                    this.RaisePropertyChanged("PollingStationNumber");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=3)]
        public string CircumscriptionName {
            get {
                return this.CircumscriptionNameField;
            }
            set {
                if ((object.ReferenceEquals(this.CircumscriptionNameField, value) != true)) {
                    this.CircumscriptionNameField = value;
                    this.RaisePropertyChanged("CircumscriptionName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=4)]
        public string CircumscriptionNumber {
            get {
                return this.CircumscriptionNumberField;
            }
            set {
                if ((object.ReferenceEquals(this.CircumscriptionNumberField, value) != true)) {
                    this.CircumscriptionNumberField = value;
                    this.RaisePropertyChanged("CircumscriptionNumber");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="getVoterDataEday.IGetDataVoters")]
    public interface IGetDataVoters {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGetDataVoters/GetVoter", ReplyAction="http://tempuri.org/IGetDataVoters/GetVoterResponse")]
        CEC.Web.Api.getVoterDataEday.VoterData GetVoter(string idnp);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGetDataVoters/GetVoter", ReplyAction="http://tempuri.org/IGetDataVoters/GetVoterResponse")]
        System.Threading.Tasks.Task<CEC.Web.Api.getVoterDataEday.VoterData> GetVoterAsync(string idnp);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IGetDataVotersChannel : CEC.Web.Api.getVoterDataEday.IGetDataVoters, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetDataVotersClient : System.ServiceModel.ClientBase<CEC.Web.Api.getVoterDataEday.IGetDataVoters>, CEC.Web.Api.getVoterDataEday.IGetDataVoters {
        
        public GetDataVotersClient() {
        }
        
        public GetDataVotersClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public GetDataVotersClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GetDataVotersClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GetDataVotersClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public CEC.Web.Api.getVoterDataEday.VoterData GetVoter(string idnp) {
            return base.Channel.GetVoter(idnp);
        }
        
        public System.Threading.Tasks.Task<CEC.Web.Api.getVoterDataEday.VoterData> GetVoterAsync(string idnp) {
            return base.Channel.GetVoterAsync(idnp);
        }
    }
}