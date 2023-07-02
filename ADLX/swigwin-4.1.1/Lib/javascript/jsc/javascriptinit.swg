%insert(init) %{
SWIGRUNTIME void
SWIG_JSC_SetModule(JSGlobalContextRef context, swig_module_info *swig_module) {
    JSObjectRef globalObject;
    JSStringRef moduleName;
    JSClassDefinition classDef;
    JSClassRef classRef;
    JSObjectRef object;

    if(context == 0){
        return;
    }

    globalObject = JSContextGetGlobalObject(context);
    moduleName = JSStringCreateWithUTF8CString("swig_module_info_data");

    classDef  = kJSClassDefinitionEmpty;
    classRef = JSClassCreate(&classDef);

    object = JSObjectMake(context, classRef, NULL);
    JSObjectSetPrivate(object, (void*)swig_module);

    JSObjectSetProperty(context, globalObject, moduleName, object, kJSPropertyAttributeNone, NULL);

    JSClassRelease(classRef);
    JSStringRelease(moduleName);
}
SWIGRUNTIME swig_module_info *
SWIG_JSC_GetModule(JSGlobalContextRef context) {
    JSObjectRef globalObject;
    JSStringRef moduleName;
    JSValueRef value;
    JSObjectRef object;

    if(context == 0){
        return 0;
    }

    globalObject = JSContextGetGlobalObject(context);
    moduleName = JSStringCreateWithUTF8CString("swig_module_info_data");

    if(JSObjectHasProperty(context, globalObject, moduleName) == false) {
        JSStringRelease(moduleName);
        return 0;
    }

    value = JSObjectGetProperty(context, globalObject, moduleName, NULL);
    object = JSValueToObject(context, value, NULL);
    JSStringRelease(moduleName);

    return (swig_module_info*)JSObjectGetPrivate(object);
}

#define SWIG_GetModule(clientdata)                SWIG_JSC_GetModule(clientdata)
#define SWIG_SetModule(clientdata, pointer)       SWIG_JSC_SetModule(clientdata, pointer)
#define SWIG_INIT_CLIENT_DATA_TYPE                JSGlobalContextRef
%}

%insert(init) "swiginit.swg"

%fragment ("js_initializer_define", "templates") %{
#define SWIGJSC_INIT $jsname_initialize
%}

// Open the initializer function
%insert(init)
%{

#ifdef __cplusplus
extern "C" {
#endif

bool SWIGJSC_INIT (JSGlobalContextRef context, JSObjectRef *exports) {
    SWIG_InitializeModule(context);
%}

/* -----------------------------------------------------------------------------
 * js_initializer:  template for the module initializer function
 *   - $jsname:                   module name
 *   - $jscreatenamespaces:       part with code for creating namespace objects
 *   - $jscreateclasses:          part with code for creating classes
 *   - $jsregisternamespaces:     part with code for registration of namespaces
 * ----------------------------------------------------------------------------- */
%fragment ("js_initializer", "templates") %{
    /* Initialize the base swig type object */
    _SwigObject_objectDefinition.staticFunctions = _SwigObject_functions;
    _SwigObject_objectDefinition.staticValues = _SwigObject_values;
    _SwigObject_classRef = JSClassCreate(&_SwigObject_objectDefinition);

    /* Initialize the PackedData class */
    _SwigPackedData_objectDefinition.staticFunctions = _SwigPackedData_functions;
    _SwigPackedData_objectDefinition.staticValues = _SwigPackedData_values;
    _SwigPackedData_objectDefinition.finalize = _wrap_SwigPackedData_delete;
    _SwigPackedData_classRef = JSClassCreate(&_SwigPackedData_objectDefinition);

    /* Create objects for namespaces */
    $jscreatenamespaces

    /* Register classes */
    $jsregisterclasses

    /* Register namespaces */
    $jsregisternamespaces

    *exports = exports_object;

    return true;
}
#ifdef __cplusplus
}
#endif
%}
