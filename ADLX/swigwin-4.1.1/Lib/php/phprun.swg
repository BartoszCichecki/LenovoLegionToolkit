/* -----------------------------------------------------------------------------
 * phprun.swg
 *
 * PHP runtime library
 * ----------------------------------------------------------------------------- */

#define swig_owntype                                    int

#ifdef __cplusplus
extern "C" {
#endif

#if PHP_MAJOR_VERSION < 7
# error These bindings need PHP 7 or later - to generate PHP5 bindings use: SWIG < 4.0.0 and swig -php5
#endif

#include "zend_inheritance.h"
#include "zend_exceptions.h"
#include "zend_inheritance.h"

#if PHP_MAJOR_VERSION == 7
/* These macros were new in PHP 8.0.  For PHP 7.x we define them to give the
 * same result except without any type declarations.  PHP 7.x supports type
 * declarations, but not for the return type, and alternate types aren't
 * supported, so we don't try to support these.
 */
# define ZEND_BEGIN_ARG_WITH_RETURN_OBJ_TYPE_MASK_EX(name, byref, num_req, classes, types) \
    ZEND_BEGIN_ARG_INFO_EX(name, 0, byref, num_req)
# define ZEND_BEGIN_ARG_WITH_RETURN_TYPE_MASK_EX(name, byref, num_req, types) \
    ZEND_BEGIN_ARG_INFO_EX(name, 0, byref, num_req)

/* NB We can just ignore `default` here we currently always pass NULL for it
 * (this mechanism for specifying default parameter values was new in PHP 8.0
 * so it's not useful while we still want to support PHP7 too).
 */
# define ZEND_ARG_OBJ_TYPE_MASK(byref, name, classes, types, default) \
    ZEND_ARG_INFO(byref, name)
# define ZEND_ARG_TYPE_MASK(byref, name, types, default) \
    ZEND_ARG_INFO(byref, name)
#endif

#include <stdlib.h> /* for abort(), used in generated code. */

#define SWIG_BOOL_CONSTANT(N, V) REGISTER_BOOL_CONSTANT(#N, V, CONST_CS | CONST_PERSISTENT)
#define SWIG_LONG_CONSTANT(N, V) REGISTER_LONG_CONSTANT(#N, V, CONST_CS | CONST_PERSISTENT)
#define SWIG_DOUBLE_CONSTANT(N, V) REGISTER_DOUBLE_CONSTANT(#N, V, CONST_CS | CONST_PERSISTENT)
#define SWIG_STRING_CONSTANT(N, V) REGISTER_STRING_CONSTANT(#N, (char*)V, CONST_CS | CONST_PERSISTENT)
#define SWIG_CHAR_CONSTANT(N, V) do {\
    char swig_char = (V);\
    REGISTER_STRINGL_CONSTANT(#N, &swig_char, 1, CONST_CS | CONST_PERSISTENT);\
} while (0)

/* ZEND_CONSTANT_SET_FLAGS was new in PHP 7.3. */
#ifdef ZEND_CONSTANT_SET_FLAGS
# define SWIG_ZEND_CONSTANT_SET_FLAGS ZEND_CONSTANT_SET_FLAGS
#else
# define SWIG_ZEND_CONSTANT_SET_FLAGS(C, F, N) do { (C)->flags = (F); (C)->module_number = (N); } while (0)
#endif

/* zend_object_alloc was new in PHP 7.3. */
#if PHP_MAJOR_VERSION == 7 && PHP_MINOR_VERSION < 3
static zend_always_inline void *zend_object_alloc(size_t obj_size, zend_class_entry *ce) {
    void *obj = emalloc(obj_size + zend_object_properties_size(ce));
    memset(obj, 0, obj_size - sizeof(zval));
    return obj;
}
#endif

/* ZEND_THIS was new in PHP 7.4. */
#ifndef ZEND_THIS
# define ZEND_THIS &EX(This)
#endif

#ifdef __cplusplus
}
#endif

#define SWIG_fail goto fail

static const char *default_error_msg = "Unknown error occurred";
static int default_error_code = E_ERROR;

#define SWIG_PHP_Arg_Error_Msg(argnum,extramsg) "Error in argument " #argnum " "#extramsg

#define SWIG_PHP_Error(code,msg) do { zend_throw_exception(NULL, msg, code); SWIG_fail; } while (0)

#define SWIG_contract_assert(expr,msg) \
  do { if (!(expr)) zend_printf("Contract Assert Failed %s\n", msg); } while (0)

/* Standard SWIG API */
#define SWIG_GetModule(clientdata) SWIG_Php_GetModule()
#define SWIG_SetModule(clientdata, pointer) SWIG_Php_SetModule(pointer, *(int*)clientdata)

static zend_class_entry SWIG_Php_swig_wrapped_interface_ce;

#if PHP_MAJOR_VERSION == 7
/* zend_class_implements_interface() was new in PHP 8.0.
 *
 * We could use instanceof_function_ex(C, I, 1) here for 7.4, but for 7.3
 * and earlier that doesn't work, so instead we just provide a compatibility
 * implementation which does what zend_class_implements_interface() does in 8.x
 * and use that for all 7.x so there are fewer variants to worry about testing.
 */
static int zend_class_implements_interface(const zend_class_entry *class_ce, const zend_class_entry *interface_ce) {
  uint32_t i;
  if (class_ce->num_interfaces) {
    for (i = 0; i < class_ce->num_interfaces; i++) {
      if (class_ce->interfaces[i] == interface_ce) {
	return 1;
      }
    }
  }
  return 0;
}
#endif

/* used to wrap returned objects in so we know whether they are newobject
   and need freeing, or not */
typedef struct {
  void * ptr;
  int newobject;
  const swig_type_info * type;
  zend_object std;
} swig_object_wrapper;

#define SWIG_Z_FETCH_OBJ_P(zv) swig_php_fetch_object(Z_OBJ_P(zv))

static inline
swig_object_wrapper * swig_php_fetch_object(zend_object *obj) {
  return (swig_object_wrapper *)((char *)obj - XtOffsetOf(swig_object_wrapper, std));
}

#define SWIG_as_voidptr(a) const_cast< void * >(static_cast< const void * >(a))

static void
SWIG_SetPointerZval(zval *z, void *ptr, swig_type_info *type, int newobject) {
  // Return PHP NULL for a C/C++ NULL pointer.
  if (!ptr) {
    ZVAL_NULL(z);
    return;
  }

  if (!type->clientdata) {
    zend_type_error("Type: %s not registered with zend", type->name);
    return;
  }

  {
    zend_object *obj;
    swig_object_wrapper *value;
    if (Z_TYPE_P(z) == IS_OBJECT) {
      /* The PHP object is already initialised - this is the case when wrapping
       * the return value from a PHP constructor. */
      obj = Z_OBJ_P(z);
    } else {
      zend_class_entry *ce = (zend_class_entry*)(type->clientdata);
      obj = ce->create_object(ce);
      ZVAL_OBJ(z, obj);
    }
    value = swig_php_fetch_object(obj);
    value->ptr = ptr;
    value->newobject = (newobject & 1);
    value->type = type;
  }
}

/* We wrap C/C++ pointers as PHP objects. */
static int
SWIG_ConvertPtrAndOwn(zval *z, void **ptr, swig_type_info *ty, int flags, swig_owntype *own) {
  if (own)
    *own = 0;

  if (z == NULL) {
    *ptr = 0;
    return (flags & SWIG_POINTER_NO_NULL) ? SWIG_NullReferenceError : SWIG_OK;
  }

  switch (Z_TYPE_P(z)) {
    case IS_OBJECT: {
      zend_object *obj = Z_OBJ_P(z);
      swig_object_wrapper *value;
      if (ty && ty->clientdata == (void*)obj->ce) {
	// Object is exactly the class asked for - this handles common cases cheaply,
	// and in particular the PHP classes we use to wrap a pointer to a non-class.
      } else if (!zend_class_implements_interface(obj->ce, &SWIG_Php_swig_wrapped_interface_ce)) {
	// Not an object we've wrapped.
	return -1;
      }

      /* convert and cast value->ptr from value->type to ptr as ty. */
      value = swig_php_fetch_object(obj);
      if (!ty) {
	/* They don't care about the target type, so just pass on the pointer! */
	*ptr = value->ptr;
      } else {
	swig_cast_info *tc = SWIG_TypeCheck(value->type->name, ty);
	if (tc) {
	  int newmemory = 0;
	  *ptr = SWIG_TypeCast(tc, value->ptr, &newmemory);
	  if (newmemory == SWIG_CAST_NEW_MEMORY) {
	    assert(own); /* badly formed typemap which will lead to a memory leak - it must set and use own to delete *ptr */
	    if (own)
	      *own |= SWIG_CAST_NEW_MEMORY;
	  }
	} else {
	  *ptr = NULL;
	}
      }

      if (((flags & SWIG_POINTER_RELEASE) == SWIG_POINTER_RELEASE) && !value->newobject) {
        return SWIG_ERROR_RELEASE_NOT_OWNED;
      } else {
        if (*ptr == NULL)
          return SWIG_ERROR; /* should be SWIG_NullReferenceError?? */
        if (flags & SWIG_POINTER_DISOWN) {
          value->newobject = 0;
        }
        if (flags & SWIG_POINTER_CLEAR) {
          value->ptr = 0;
        }
      }

      return SWIG_OK;
    }
    case IS_NULL:
      *ptr = 0;
      return (flags & SWIG_POINTER_NO_NULL) ? SWIG_NullReferenceError : SWIG_OK;
  }

  return -1;
}

static int
SWIG_ConvertPtr(zval *z, void **ptr, swig_type_info *ty, int flags) {
  return SWIG_ConvertPtrAndOwn(z, ptr, ty, flags, 0);
}

static const char const_name[] = "swig_runtime_data_type_pointer";
static swig_module_info *SWIG_Php_GetModule(void) {
  zval *pointer = zend_get_constant_str(const_name, sizeof(const_name) - 1);
  if (pointer) {
    if (Z_TYPE_P(pointer) == IS_LONG) {
      return (swig_module_info *) pointer->value.lval;
    }
  }
  return NULL;
}

static void SWIG_Php_SetModule(swig_module_info *pointer, int module_number) {
  REGISTER_LONG_CONSTANT(const_name, (long) pointer, CONST_CS | CONST_PERSISTENT);
}

/* Common parts of the "create_object" object handler. */
static zend_object *SWIG_Php_do_create_object(zend_class_entry *ce, zend_object_handlers *handlers) {
  swig_object_wrapper *obj = (swig_object_wrapper*)zend_object_alloc(sizeof(swig_object_wrapper), ce);
  zend_object_std_init(&obj->std, ce);
  object_properties_init(&obj->std, ce);
  obj->std.handlers = handlers;
  obj->newobject = 1;
  return &obj->std;
}

/* Common parts of the "free_obj" object handler.
   Returns void* pointer if the C/C++ object should be destroyed. */
static void* SWIG_Php_free_obj(zend_object *object) {
  if (object) {
    swig_object_wrapper *obj = swig_php_fetch_object(object);
    zend_object_std_dtor(&obj->std);
    if (obj->newobject) return obj->ptr;
  }
  return NULL;
}
