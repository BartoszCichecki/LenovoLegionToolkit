/* -----------------------------------------------------------------------------
 * js_ctor:  template for wrapping a ctor.
 *   - $jswrapper:        wrapper of called ctor
 *   - $jslocals:         locals part of wrapper
 *   - $jscode:           code part of wrapper
 *   - $jsargcount:       number of arguments
 *   - $jsmangledtype:    mangled type of class
 * ----------------------------------------------------------------------------- */

%fragment("js_ctor", "templates") %{
static SwigV8ReturnValue $jswrapper(const SwigV8Arguments &args) {
  SWIGV8_HANDLESCOPE();

  SWIGV8_OBJECT self = args.Holder();
  $jslocals
  if(self->InternalFieldCount() < 1) SWIG_exception_fail(SWIG_ERROR, "Illegal call of constructor $jswrapper.");
  if(args.Length() != $jsargcount) SWIG_exception_fail(SWIG_ERROR, "Illegal number of arguments for $jswrapper.");
  $jscode

  SWIGV8_SetPrivateData(self, result, SWIGTYPE_$jsmangledtype, SWIG_POINTER_OWN);
  SWIGV8_RETURN(self);

  goto fail;
fail:
  SWIGV8_RETURN(SWIGV8_UNDEFINED());
}
%}

/* -----------------------------------------------------------------------------
 * js_veto_ctor:  a vetoing ctor for abstract classes
 *   - $jswrapper:        name of wrapper
 *   - $jsname:           class name
 * ----------------------------------------------------------------------------- */
%fragment ("js_veto_ctor", "templates")
%{
static SwigV8ReturnValue $jswrapper(const SwigV8Arguments &args) {
  SWIGV8_HANDLESCOPE();
  
  SWIG_exception(SWIG_ERROR, "Class $jsname can not be instantiated");
fail:
  SWIGV8_RETURN(SWIGV8_UNDEFINED());
}
%}

/* -----------------------------------------------------------------------------
 * js_ctor_dispatcher:  dispatcher for overloaded constructors
 *   - $jswrapper:        name of wrapper
 *   - $jsname:           class name
 *   - $jsdispatchcases:  part containing code for dispatching
 * ----------------------------------------------------------------------------- */
%fragment ("js_ctor_dispatcher", "templates")
%{
static SwigV8ReturnValue $jswrapper(const SwigV8Arguments &args) {
  SWIGV8_HANDLESCOPE();
  
  OverloadErrorHandler errorHandler;
  SWIGV8_VALUE self;

  // switch all cases by means of series of if-returns.
  $jsdispatchcases

  // default:
  SWIG_exception_fail(SWIG_ERROR, "Illegal arguments for construction of $jsmangledname");

fail:
  SWIGV8_RETURN(SWIGV8_UNDEFINED());
}
%}

/* -----------------------------------------------------------------------------
 * js_overloaded_ctor:  template for wrapping a ctor.
 *   - $jswrapper:        wrapper of called ctor
 *   - $jslocals:         locals part of wrapper
 *   - $jscode:           code part of wrapper
 *   - $jsargcount:       number of arguments
 *   - $jsmangledtype:    mangled type of class
 * ----------------------------------------------------------------------------- */
%fragment("js_overloaded_ctor", "templates") %{
static SwigV8ReturnValue $jswrapper(const SwigV8Arguments &args, V8ErrorHandler &SWIGV8_ErrorHandler) {
  SWIGV8_HANDLESCOPE();

  SWIGV8_OBJECT self = args.Holder();
  $jslocals
  if(self->InternalFieldCount() < 1) SWIG_exception_fail(SWIG_ERROR, "Illegal call of constructor $jswrapper.");
  if(args.Length() != $jsargcount) SWIG_exception_fail(SWIG_ERROR, "Illegal number of arguments for $jswrapper.");
  $jscode

  SWIGV8_SetPrivateData(self, result, SWIGTYPE_$jsmangledtype, SWIG_POINTER_OWN);
  SWIGV8_RETURN(self);

  goto fail;
fail:
  SWIGV8_RETURN(SWIGV8_UNDEFINED());
}
%}

/* -----------------------------------------------------------------------------
 * js_ctor_dispatch_case:  template for a dispatch case for calling an overloaded ctor.
 *   - $jsargcount:       number of arguments of called ctor
 *   - $jswrapper:        wrapper of called ctor
 *
 *  Note: a try-catch-like mechanism is used to switch cases
 * ----------------------------------------------------------------------------- */
%fragment ("js_ctor_dispatch_case", "templates")
%{
  if(args.Length() == $jsargcount) {
    errorHandler.err.Clear();
    $jswrapper(args, errorHandler);
    if(errorHandler.err.IsEmpty()) {
      return;
    }
  }
%}

/* -----------------------------------------------------------------------------
 * js_dtor:  template for a destructor wrapper
 *   - $jsmangledname:  mangled class name
 *   - $jstype:         class type
 * ----------------------------------------------------------------------------- */
%fragment ("js_dtor", "templates")
%{

static void $jswrapper(const v8::WeakCallbackInfo<SWIGV8_Proxy> &data) {
  SWIGV8_Proxy *proxy = data.GetParameter();

  if(proxy->swigCMemOwn && proxy->swigCObject) {
#ifdef SWIGRUNTIME_DEBUG
    printf("Deleting wrapped instance: %s\n", proxy->info->name);
#endif
    $jsfree proxy->swigCObject;
  }
  delete proxy;
}
%}

/* -----------------------------------------------------------------------------
 * js_dtoroverride:  template for a destructor wrapper
 *   - $jsmangledname:  mangled class name
 *   - $jstype:         class type
 *   - ${destructor_action}: The custom destructor action to invoke.
 * ----------------------------------------------------------------------------- */
%fragment ("js_dtoroverride", "templates")
%{
static void $jswrapper(const v8::WeakCallbackInfo<SWIGV8_Proxy> &data) {
  SWIGV8_Proxy *proxy = data.GetParameter();

  if(proxy->swigCMemOwn && proxy->swigCObject) {
    $jstype arg1 = ($jstype)proxy->swigCObject;
    ${destructor_action}
  }
  delete proxy;
}
%}

/* -----------------------------------------------------------------------------
 * js_getter:  template for getter function wrappers
 *   - $jswrapper:  wrapper function name
 *   - $jslocals:   locals part of wrapper
 *   - $jscode:     code part of wrapper
 * ----------------------------------------------------------------------------- */
%fragment("js_getter", "templates")
%{
static SwigV8ReturnValue $jswrapper(v8::Local<v8::Name> property, const SwigV8PropertyCallbackInfo &info) {
  SWIGV8_HANDLESCOPE();
  
  SWIGV8_VALUE jsresult;
  $jslocals
  $jscode
  SWIGV8_RETURN_INFO(jsresult, info);

  goto fail;
fail:
  SWIGV8_RETURN_INFO(SWIGV8_UNDEFINED(), info);
}
%}

/* -----------------------------------------------------------------------------
 * js_setter:  template for setter function wrappers
 *   - $jswrapper:  wrapper function name
 *   - $jslocals:   locals part of wrapper
 *   - $jscode:     code part of wrapper
 * ----------------------------------------------------------------------------- */
%fragment("js_setter", "templates")
%{
static void $jswrapper(v8::Local<v8::Name> property, v8::Local<v8::Value> value, const SwigV8PropertyCallbackInfoVoid &info) {
  SWIGV8_HANDLESCOPE();
  
  $jslocals
  $jscode
  goto fail;
fail:
  return;
}
%}

/* -----------------------------------------------------------------------------
 * js_function:  template for function wrappers
 *   - $jswrapper:  wrapper function name
 *   - $jslocals:   locals part of wrapper
 *   - $jscode:     code part of wrapper
 * ----------------------------------------------------------------------------- */
%fragment("js_function", "templates")
%{
static SwigV8ReturnValue $jswrapper(const SwigV8Arguments &args) {
  SWIGV8_HANDLESCOPE();
  
  SWIGV8_VALUE jsresult;
  $jslocals
  if(args.Length() != $jsargcount) SWIG_exception_fail(SWIG_ERROR, "Illegal number of arguments for $jswrapper.");

  $jscode
  SWIGV8_RETURN(jsresult);

  goto fail;
fail:
  SWIGV8_RETURN(SWIGV8_UNDEFINED());
}
%}

/* -----------------------------------------------------------------------------
 * js_function_dispatcher:  template for a function dispatcher for overloaded functions
 *   - $jswrapper:  wrapper function name
 *   - $jsname:     name of the wrapped function
 *   - $jslocals:   locals part of wrapper
 *   - $jscode:     code part of wrapper
 * ----------------------------------------------------------------------------- */
%fragment("js_function_dispatcher", "templates")
%{
static SwigV8ReturnValue $jswrapper(const SwigV8Arguments &args) {
  SWIGV8_HANDLESCOPE();
  
  SWIGV8_VALUE jsresult;
  OverloadErrorHandler errorHandler;
  $jscode

  SWIG_exception_fail(SWIG_ERROR, "Illegal arguments for function $jsname.");

  goto fail;
fail:
  SWIGV8_RETURN(SWIGV8_UNDEFINED());
}
%}

/* -----------------------------------------------------------------------------
 * js_overloaded_function:  template for a overloaded function
 *   - $jswrapper:  wrapper function name
 *   - $jslocals:   locals part of wrapper
 *   - $jscode:     code part of wrapper
 * ----------------------------------------------------------------------------- */
%fragment ("js_overloaded_function", "templates")
%{
static SwigV8ReturnValue $jswrapper(const SwigV8Arguments &args, V8ErrorHandler &SWIGV8_ErrorHandler)
{
  SWIGV8_HANDLESCOPE();
  
  SWIGV8_VALUE jsresult;
  $jslocals
  $jscode
  SWIGV8_RETURN(jsresult);

  goto fail;
fail:
  SWIGV8_RETURN(SWIGV8_UNDEFINED());
}
%}

/* -----------------------------------------------------------------------------
 * js_function_dispatch_case:  template for a case used in the function dispatcher
 *   - $jswrapper:  wrapper function name
 *   - $jsargcount: number of arguments of overloaded function
 *   - $jscode:     code part of wrapper
 * ----------------------------------------------------------------------------- */
%fragment ("js_function_dispatch_case", "templates")
%{

  if(args.Length() == $jsargcount) {
    errorHandler.err.Clear();
    $jswrapper(args, errorHandler);
    if(errorHandler.err.IsEmpty()) {
      return;
    }
  }
%}

/* -----------------------------------------------------------------------------
 * jsv8_declare_class_template:  template for a class template declaration.
 *   - $jsmangledname:  mangled class name
 * ----------------------------------------------------------------------------- */
%fragment("jsv8_declare_class_template", "templates")
%{
  SWIGV8_ClientData $jsmangledname_clientData;
%}

/* -----------------------------------------------------------------------------
 * jsv8_define_class_template:  template for a class template definition.
 *   - $jsmangledname:  mangled class name
 *   - $jsmangledtype:  mangled class type
 *   - $jsdtor:         the dtor wrapper
 * ----------------------------------------------------------------------------- */
%fragment("jsv8_define_class_template", "templates")
%{
  /* Name: $jsmangledname, Type: $jsmangledtype, Dtor: $jsdtor */
  SWIGV8_FUNCTION_TEMPLATE $jsmangledname_class = SWIGV8_CreateClassTemplate("$jsmangledname");
  SWIGV8_SET_CLASS_TEMPL($jsmangledname_clientData.class_templ, $jsmangledname_class);
  $jsmangledname_clientData.dtor = $jsdtor;
  if (SWIGTYPE_$jsmangledtype->clientdata == 0) {
    SWIGTYPE_$jsmangledtype->clientdata = &$jsmangledname_clientData;
  }
%}


/* -----------------------------------------------------------------------------
 * jsv8_inherit:  template for an class inherit statement.
 *   - $jsmangledname:  mangled class name
 *   - $jsbaseclass:    mangled name of the base class
 * ----------------------------------------------------------------------------- */
%fragment("jsv8_inherit", "templates")
%{
  if (SWIGTYPE_p$jsbaseclass->clientdata && !(static_cast<SWIGV8_ClientData *>(SWIGTYPE_p$jsbaseclass->clientdata)->class_templ.IsEmpty()))
  {
    $jsmangledname_class->Inherit(
      v8::Local<v8::FunctionTemplate>::New(
        v8::Isolate::GetCurrent(),
        static_cast<SWIGV8_ClientData *>(SWIGTYPE_p$jsbaseclass->clientdata)->class_templ)
     );

#ifdef SWIGRUNTIME_DEBUG
    printf("Inheritance successful $jsmangledname $jsbaseclass\n");
#endif
  } else {
#ifdef SWIGRUNTIME_DEBUG
    printf("Unable to inherit baseclass, it didn't exist $jsmangledname $jsbaseclass\n");
#endif
  }
%}

/* -----------------------------------------------------------------------------
 * jsv8_create_class_instance:  template for creating an class object.
 *   - $jsname:         class name
 *   - $jsmangledname:  mangled class name
 * ----------------------------------------------------------------------------- */
%fragment("jsv8_create_class_instance", "templates")
%{
  /* Class: $jsname ($jsmangledname) */
  SWIGV8_FUNCTION_TEMPLATE $jsmangledname_class_0 = SWIGV8_CreateClassTemplate("$jsname");
  $jsmangledname_class_0->SetCallHandler($jsctor);
  $jsmangledname_class_0->Inherit($jsmangledname_class);
#if (SWIG_V8_VERSION < 0x0704)
  $jsmangledname_class_0->SetHiddenPrototype(true);
  v8::Local<v8::Object> $jsmangledname_obj = $jsmangledname_class_0->GetFunction();
#else
  v8::Local<v8::Object> $jsmangledname_obj = $jsmangledname_class_0->GetFunction(context).ToLocalChecked();
#endif
%}

/* -----------------------------------------------------------------------------
 * jsv8_register_class:  template for a statement that registers a class in a parent namespace.
 *   - $jsname:         class name
 *   - $jsmangledname:  mangled class name
 *   - $jsparent:       mangled name of parent namespace
 * ----------------------------------------------------------------------------- */
%fragment("jsv8_register_class", "templates")
%{
  SWIGV8_MAYBE_CHECK($jsparent_obj->Set(context, SWIGV8_SYMBOL_NEW("$jsname"), $jsmangledname_obj));
%}

/* -----------------------------------------------------------------------------
 * jsv8_create_namespace:  template for a statement that creates a namespace object.
 *   - $jsmangledname:  mangled namespace name
 * ----------------------------------------------------------------------------- */
%fragment("jsv8_create_namespace", "templates")
%{
  SWIGV8_OBJECT $jsmangledname_obj = SWIGV8_OBJECT_NEW();
%}

/* -----------------------------------------------------------------------------
 * jsv8_register_namespace:  template for a statement that registers a namespace in a parent namespace.
 *   - $jsname:         name of namespace
 *   - $jsmangledname:  mangled name of namespace
 *   - $jsparent:       mangled name of parent namespace
 * ----------------------------------------------------------------------------- */
%fragment("jsv8_register_namespace", "templates")
%{
  SWIGV8_MAYBE_CHECK($jsparent_obj->Set(context, SWIGV8_SYMBOL_NEW("$jsname"), $jsmangledname_obj));
%}

/* -----------------------------------------------------------------------------
 * jsv8_register_member_function:  template for a statement that registers a member function.
 *   - $jsmangledname:  mangled class name
 *   - $jsname:         name of the function
 *   - $jswrapper:      wrapper of the member function
 * ----------------------------------------------------------------------------- */
%fragment("jsv8_register_member_function", "templates")
%{
  SWIGV8_AddMemberFunction($jsmangledname_class, "$jsname", $jswrapper);
%}

/* -----------------------------------------------------------------------------
 * jsv8_register_member_variable:  template for a statement that registers a member variable.
 *   - $jsmangledname:  mangled class name
 *   - $jsname:         name of the function
 *   - $jsgetter:       wrapper of the getter function
 *   - $jssetter:       wrapper of the setter function
 * ----------------------------------------------------------------------------- */
%fragment("jsv8_register_member_variable", "templates")
%{
  SWIGV8_AddMemberVariable($jsmangledname_class, "$jsname", $jsgetter, $jssetter);
%}

/* -----------------------------------------------------------------------------
 * jsv8_register_static_function:  template for a statement that registers a static class function.
 *   - $jsname:         function name
 *   - $jswrapper:      wrapper of the function
 *   - $jsparent:       mangled name of parent namespace
 *
 * Note: this template is also used for global functions.
 * ----------------------------------------------------------------------------- */
%fragment("jsv8_register_static_function", "templates")
%{
  SWIGV8_AddStaticFunction($jsparent_obj, "$jsname", $jswrapper, context);
%}

/* -----------------------------------------------------------------------------
 * jsv8_register_static_variable:  template for a statement that registers a static variable.
 *   - $jsname:         variable name
 *   - $jsparent:       mangled name of parent namespace
 *   - $jsgetter:       wrapper of the getter function
 *   - $jssetter:       wrapper of the setter function
 *
 * Note: this template is also used for global variables.
 * ----------------------------------------------------------------------------- */
%fragment("jsv8_register_static_variable", "templates")
%{
  SWIGV8_AddStaticVariable($jsparent_obj, "$jsname", $jsgetter, $jssetter, context);
%}
