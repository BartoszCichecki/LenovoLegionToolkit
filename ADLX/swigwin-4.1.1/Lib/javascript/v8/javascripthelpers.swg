%insert(runtime) %{

typedef v8::FunctionCallback            SwigV8FunctionCallback;
typedef v8::AccessorNameGetterCallback  SwigV8AccessorGetterCallback;
typedef v8::AccessorNameSetterCallback  SwigV8AccessorSetterCallback;
typedef v8::PropertyCallbackInfo<void>  SwigV8PropertyCallbackInfoVoid;

/**
 * Creates a class template for a class with specified initialization function.
 */
SWIGRUNTIME SWIGV8_FUNCTION_TEMPLATE SWIGV8_CreateClassTemplate(const char* symbol) {
    SWIGV8_HANDLESCOPE_ESC();
    
    v8::Local<v8::FunctionTemplate> class_templ = SWIGV8_FUNCTEMPLATE_NEW_VOID();
    class_templ->SetClassName(SWIGV8_SYMBOL_NEW(symbol));

    SWIGV8_OBJECT_TEMPLATE inst_templ = class_templ->InstanceTemplate();
    inst_templ->SetInternalFieldCount(1);

    SWIGV8_OBJECT_TEMPLATE equals_templ = class_templ->PrototypeTemplate();
    equals_templ->Set(SWIGV8_SYMBOL_NEW("equals"), SWIGV8_FUNCTEMPLATE_NEW(_SWIGV8_wrap_equals));

    SWIGV8_OBJECT_TEMPLATE cptr_templ = class_templ->PrototypeTemplate();
    cptr_templ->Set(SWIGV8_SYMBOL_NEW("getCPtr"), SWIGV8_FUNCTEMPLATE_NEW(_wrap_getCPtr));

    SWIGV8_ESCAPE(class_templ);
}

/**
 * Registers a class method with given name for a given class template.
 */
SWIGRUNTIME void SWIGV8_AddMemberFunction(SWIGV8_FUNCTION_TEMPLATE class_templ, const char* symbol,
  SwigV8FunctionCallback _func) {
    SWIGV8_OBJECT_TEMPLATE proto_templ = class_templ->PrototypeTemplate();
    proto_templ->Set(SWIGV8_SYMBOL_NEW(symbol), SWIGV8_FUNCTEMPLATE_NEW(_func));
}

/**
 * Registers a class property with given name for a given class template.
 */
SWIGRUNTIME void SWIGV8_AddMemberVariable(SWIGV8_FUNCTION_TEMPLATE class_templ, const char* symbol,
  SwigV8AccessorGetterCallback getter, SwigV8AccessorSetterCallback setter) {
  SWIGV8_OBJECT_TEMPLATE proto_templ = class_templ->InstanceTemplate();
  proto_templ->SetAccessor(SWIGV8_SYMBOL_NEW(symbol), getter, setter);
}

/**
 * Registers a class method with given name for a given object.
 */
SWIGRUNTIME void SWIGV8_AddStaticFunction(SWIGV8_OBJECT obj, const char* symbol,
  const SwigV8FunctionCallback& _func, v8::Local<v8::Context> context) {
  SWIGV8_MAYBE_CHECK(obj->Set(context, SWIGV8_SYMBOL_NEW(symbol), SWIGV8_FUNCTEMPLATE_NEW(_func)->GetFunction(context).ToLocalChecked()));
}

/**
 * Registers a class method with given name for a given object.
 */
SWIGRUNTIME void SWIGV8_AddStaticVariable(SWIGV8_OBJECT obj, const char* symbol,
  SwigV8AccessorGetterCallback getter, SwigV8AccessorSetterCallback setter,
  v8::Local<v8::Context> context) {
  SWIGV8_MAYBE_CHECK(obj->SetAccessor(context, SWIGV8_SYMBOL_NEW(symbol), getter, setter));
}

SWIGRUNTIME void JS_veto_set_variable(v8::Local<v8::Name> property, v8::Local<v8::Value> value, const SwigV8PropertyCallbackInfoVoid& info)
{
    char buffer[256];
    char msg[512];
    int res;

    v8::Local<v8::String> sproperty;
    if (property->ToString(SWIGV8_CURRENT_CONTEXT()).ToLocal(&sproperty)) {
      SWIGV8_WRITE_UTF8(sproperty, buffer, 256);
      res = sprintf(msg, "Tried to write read-only variable: %s.", buffer);
    }
    else {
      res = -1;
    }

    if(res<0) {
      SWIG_exception(SWIG_ERROR, "Tried to write read-only variable.");
    } else {
      SWIG_exception(SWIG_ERROR, msg);
    }
fail: ;
}

%} // v8_helper_functions
