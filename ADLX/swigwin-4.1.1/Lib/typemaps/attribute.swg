/* -----------------------------------------------------------------------------
 * attribute.swg
 *
 * Attribute implementation
 * ----------------------------------------------------------------------------- */

/*
  The following macros convert a pair of set/get methods
  into a "native" attribute.
*/

//
// Define SWIG_ATTRIBUTE_TEMPLATE if you want to use templates instead of macros for the C++ get and set wrapper methods
// Does not always generate compilable code, use at your peril!
//
//#define SWIG_ATTRIBUTE_TEMPLATE

%define %attribute_custom(Class, AttributeType, AttributeName, GetMethod, SetMethod, GetMethodCall, SetMethodCall)
  %ignore Class::GetMethod();
  %ignore Class::GetMethod() const;
  #if #SetMethod != #AttributeName
    %ignore Class::SetMethod;
  #endif
  %extend Class {
    AttributeType AttributeName;
  }
#if defined(__cplusplus) && defined(SWIG_ATTRIBUTE_TEMPLATE)
  %{
    template < class C > inline AttributeType %mangle(Class) ##_## AttributeName ## _get(const C* self_) {
      return GetMethodCall;
    }
    template < class C > inline AttributeType %mangle(Class) ##_## AttributeName ## _get(C* self_) {
      return GetMethodCall;
    }
    template < class C > inline void %mangle(Class) ##_## AttributeName ## _set(C* self_, AttributeType val_) {
      SetMethodCall;
    }
  %}
#else
  %{
    #define %mangle(Class) ##_## AttributeName ## _get(self_) GetMethodCall
    #define %mangle(Class) ##_## AttributeName ## _set(self_, val_) SetMethodCall
  %}
#endif
%enddef

%define %attribute_readonly(Class, AttributeType, AttributeName, GetMethod, GetMethodCall)
  %ignore Class::GetMethod();
  %ignore Class::GetMethod() const;
  %immutable Class::AttributeName;
  %extend Class {
    AttributeType AttributeName;
  }
#if defined(__cplusplus) && defined(SWIG_ATTRIBUTE_TEMPLATE)
  %{
    template < class C > inline AttributeType %mangle(Class) ##_## AttributeName ## _get(const C* self_) {
      return GetMethodCall;
    }
    template < class C > inline AttributeType %mangle(Class) ##_## AttributeName ## _get(C* self_) {
      return GetMethodCall;
    }
  %}
#else
  %{
    #define %mangle(Class) ##_## AttributeName ## _get(self_) GetMethodCall
  %}
#endif
%enddef


// User macros

%define %attribute(Class, AttributeType, AttributeName, GetMethod, SetMethod...)
  #if #SetMethod != ""
    %attribute_custom(%arg(Class), %arg(AttributeType), AttributeName, GetMethod, SetMethod, self_->GetMethod(), self_->SetMethod(val_))
  #else
    %attribute_readonly(%arg(Class), %arg(AttributeType), AttributeName, GetMethod, self_->GetMethod())
  #endif
%enddef

%define %attribute2(Class, AttributeType, AttributeName, GetMethod, SetMethod...)
  #if #SetMethod != ""
    %attribute_custom(%arg(Class), %arg(AttributeType), AttributeName, GetMethod, SetMethod, &self_->GetMethod(), self_->SetMethod(*val_))
  #else
    %attribute_readonly(%arg(Class), %arg(AttributeType), AttributeName, GetMethod, &self_->GetMethod())
  #endif
%enddef

%define %attributeref(Class, AttributeType, AttributeName, AccessorMethod...)
  #if #AccessorMethod != ""
    %attribute_custom(%arg(Class), %arg(AttributeType), AttributeName, AccessorMethod, AccessorMethod, self_->AccessorMethod(), self_->AccessorMethod() = val_)
  #else
    %attribute_custom(%arg(Class), %arg(AttributeType), AttributeName, AttributeName, AttributeName, self_->AttributeName(), self_->AttributeName() = val_)
  #endif
%enddef

%define %attribute2ref(Class, AttributeType, AttributeName, AccessorMethod...)
  #if #AccessorMethod != ""
    %attribute_custom(%arg(Class), %arg(AttributeType), AttributeName, AccessorMethod, AccessorMethod, &self_->AccessorMethod(), self_->AccessorMethod() = *val_)
  #else
    %attribute_custom(%arg(Class), %arg(AttributeType), AttributeName, AttributeName, AttributeName, &self_->AttributeName(), self_->AttributeName() = *val_)
  #endif
%enddef

// deprecated (same as %attributeref, but there is an argument order inconsistency)
%define %attribute_ref(Class, AttributeType, AccessorMethod, AttributeName...)
  #if #AttributeName != ""
    %attribute_custom(%arg(Class), %arg(AttributeType), AttributeName, AccessorMethod, AccessorMethod, self_->AccessorMethod(), self_->AccessorMethod() = val_)
  #else
    %attribute_custom(%arg(Class), %arg(AttributeType), AccessorMethod, AccessorMethod, AccessorMethod, self_->AccessorMethod(), self_->AccessorMethod() = val_)
  #endif
%enddef


%define %attributeval(Class, AttributeType, AttributeName, GetMethod, SetMethod...)
  %{
    #define %mangle(Class) ##_## AttributeName ## _get(self_) new AttributeType(self_->GetMethod())
  %}
  #if #SetMethod != ""
    %{
      #define %mangle(Class) ##_## AttributeName ## _set(self_, val_) self_->SetMethod(*val_)
    %}
    #if #SetMethod != #AttributeName
      %ignore Class::SetMethod;
    #endif
  #else
    %immutable Class::AttributeName;
  #endif
  %ignore Class::GetMethod();
  %ignore Class::GetMethod() const;
  %newobject Class::AttributeName;
  %extend Class {
    AttributeType AttributeName;
  }
%enddef


%define %attributestring(Class, AttributeType, AttributeName, GetMethod, SetMethod...)
  %{
    #define %mangle(Class) ##_## AttributeName ## _get(self_) *new AttributeType(self_->GetMethod())
  %}
  #if #SetMethod != ""
    %{
      #define %mangle(Class) ##_## AttributeName ## _set(self_, val_) self_->SetMethod(val_)
    %}
    #if #SetMethod != #AttributeName
      %ignore Class::SetMethod;
    #endif
  #else
    %immutable Class::AttributeName;
  #endif
  %ignore Class::GetMethod();
  %ignore Class::GetMethod() const;
  %newobject Class::AttributeName;
  %typemap(newfree) const AttributeType &AttributeName "delete $1;"
  %extend Class {
    AttributeType AttributeName;
  }
%enddef

