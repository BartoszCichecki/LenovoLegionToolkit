%insert(header) %{
#include <assert.h>
%}

%insert(init) %{

SWIGRUNTIME void
SWIG_V8_SetModule(v8::Local<v8::Context> context, swig_module_info *swig_module) {
  v8::Local<v8::Object> global_obj = context->Global();
  v8::Local<v8::External> mod = SWIGV8_EXTERNAL_NEW(swig_module);
  assert(!mod.IsEmpty());
  v8::Local<v8::Private> privateKey = v8::Private::ForApi(v8::Isolate::GetCurrent(), SWIGV8_STRING_NEW("swig_module_info_data"));
  global_obj->SetPrivate(context, privateKey, mod);
}

SWIGRUNTIME swig_module_info *
SWIG_V8_GetModule(v8::Local<v8::Context> context) {
  v8::Local<v8::Object> global_obj = context->Global();
  v8::Local<v8::Private> privateKey = v8::Private::ForApi(v8::Isolate::GetCurrent(), SWIGV8_STRING_NEW("swig_module_info_data"));
  v8::Local<v8::Value> moduleinfo;
  if (!global_obj->GetPrivate(context, privateKey).ToLocal(&moduleinfo))
    return 0;

  if (moduleinfo.IsEmpty() || moduleinfo->IsNull() || moduleinfo->IsUndefined())
  {
    // It's not yet loaded
    return 0;
  }

  v8::Local<v8::External> moduleinfo_extern = v8::Local<v8::External>::Cast(moduleinfo);

  if (moduleinfo_extern.IsEmpty() || moduleinfo_extern->IsNull() || moduleinfo_extern->IsUndefined())
  {
    // Something's not right
    return 0;
  }

  void *ptr = moduleinfo_extern->Value();
  assert(ptr);
  swig_module_info *retptr = static_cast<swig_module_info *>(ptr);
  assert(retptr);
  return retptr;
}

#define SWIG_GetModule(clientdata)                SWIG_V8_GetModule(clientdata)
#define SWIG_SetModule(clientdata, pointer)       SWIG_V8_SetModule(clientdata, pointer)
#define SWIG_INIT_CLIENT_DATA_TYPE                v8::Local<v8::Context>

%}

%insert(init) "swiginit.swg"

// Open the initializer function definition here

%fragment ("js_initializer_define", "templates") %{
#define SWIGV8_INIT $jsname_initialize
%}

%insert(init) %{
#if !defined(NODE_MODULE_VERSION) || (NODE_MODULE_VERSION < 12)
// Note: 'extern "C"'' disables name mangling which makes it easier to load the symbol manually
extern "C" void SWIGV8_INIT (SWIGV8_OBJECT exports_obj)
#elif (NODE_MODULE_VERSION < 64)
void SWIGV8_INIT (SWIGV8_OBJECT exports_obj, SWIGV8_VALUE /*module*/, void*)
#else
void SWIGV8_INIT (SWIGV8_OBJECT exports_obj, SWIGV8_VALUE /*module*/, v8::Local<v8::Context> context, void*)
#endif
{
#if !defined(NODE_MODULE_VERSION) || NODE_MODULE_VERSION < 64
  v8::Local<v8::Context> context = SWIGV8_CURRENT_CONTEXT();
#endif

  SWIG_InitializeModule(context);
%}


/* -----------------------------------------------------------------------------
 * js_initializer:  template for the module initializer function
 *   - $jsname:                   module name
 *   - $jsv8nspaces:              part with code creating namespace objects
 *   - $jsv8classtemplates:       part with code creating class templates
 *   - $jsv8wrappers:             part with code that registers wrapper functions
 *   - $jsv8inheritance:          part with inherit statements
 *   - $jsv8classinstances:       part with code creating class objects
 *   - $jsv8staticwrappers:       part with code adding static functions to class objects
 *   - $jsv8registerclasses:      part with code that registers class objects in namespaces
 *   - $jsv8registernspaces:      part with code that registers namespaces in parent namespaces
 * ----------------------------------------------------------------------------- */
%fragment("js_initializer", "templates")
%{
  // a class template for creating proxies of undefined types
  SWIGV8_SET_CLASS_TEMPL(SWIGV8_SWIGTYPE_Proxy_class_templ, SWIGV8_CreateClassTemplate("SwigProxy"));

  /* create objects for namespaces */
  $jsv8nspaces

  /* create class templates */
  $jsv8classtemplates

  /* register wrapper functions */
  $jsv8wrappers

  /* setup inheritances */
  $jsv8inheritance

  /* class instances */
  $jsv8classinstances

  /* add static class functions and variables */
  $jsv8staticwrappers

  /* register classes */
  $jsv8registerclasses

  /* create and register namespace objects */
  $jsv8registernspaces
}

#if defined(BUILDING_NODE_EXTENSION)
#if (NODE_MODULE_VERSION < 64)
NODE_MODULE($jsname, $jsname_initialize)
#else
NODE_MODULE_CONTEXT_AWARE($jsname, $jsname_initialize)
#endif
#endif
%}
